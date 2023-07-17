import cv2
import numpy as np
import json

SMOOTHING_RADIUS = 100

def filter_contours(input_contours, min_area, min_perimeter, min_width=0, max_width=500,
                    min_height=0, max_height=500, solidity=(0,100), max_vertex_count=500, min_vertex_count=0,
                    min_ratio=0, max_ratio=500):
    """Filters out contours that do not meet certain criteria.
    Args:
        input_contours: Contours as a list of numpy.ndarray.
        min_area: The minimum area of a contour that will be kept.
        min_perimeter: The minimum perimeter of a contour that will be kept.
        min_width: Minimum width of a contour.
        max_width: MaxWidth maximum width.
        min_height: Minimum height.
        max_height: Maximimum height.
        solidity: The minimum and maximum solidity of a contour.
        min_vertex_count: Minimum vertex Count of the contours.
        max_vertex_count: Maximum vertex Count.
        min_ratio: Minimum ratio of width to height.
        max_ratio: Maximum ratio of width to height.
    Returns:
        Contours as a list of numpy.ndarray.
    """
    output = []
    for contour in input_contours:
        x,y,w,h = cv2.boundingRect(contour)
        if (w < min_width or w > max_width):
            continue
        if (h < min_height or h > max_height):
            continue
        area = cv2.contourArea(contour)
        if (area < min_area):
            continue
        if (cv2.arcLength(contour, True) < min_perimeter):
            continue
        hull = cv2.convexHull(contour)
        solid = 100 * area / cv2.contourArea(hull)
        if (solid < solidity[0] or solid > solidity[1]):
            continue
        if (len(contour) < min_vertex_count or len(contour) > max_vertex_count):
            continue
        ratio = (float)(w) / h
        if (ratio < min_ratio or ratio > max_ratio):
            continue
        output.append(contour)
    return output


def fixBorder(frame):
  s = frame.shape
  # Scale the image 4% without moving the center
  T = cv2.getRotationMatrix2D((s[1]/2, s[0]/2), 0, 1.04)
  frame = cv2.warpAffine(frame, T, (s[1], s[0]))
  return frame


def movingAverage(curve, radius):
  window_size = 2 * radius + 1
  # Define the filter
  f = np.ones(window_size)/window_size
  # Add padding to the boundaries
  curve_pad = np.lib.pad(curve, (radius, radius), 'edge')
  # Apply convolution
  curve_smoothed = np.convolve(curve_pad, f, mode='same')
  # Remove padding
  curve_smoothed = curve_smoothed[radius:-radius]
  # return smoothed curve
  return curve_smoothed


def smooth(trajectory):
    smoothed_trajectory = np.copy(trajectory)
    # Filter the x, y and angle curves
    for i in range(3):
        smoothed_trajectory[:, i] = movingAverage(trajectory[:, i], radius=SMOOTHING_RADIUS)

    return smoothed_trajectory

def preprocessing(cap, w, h, total_frame_count):
    # Read the first frame
    _, previous_frame = cap.read()
    previous_frame = previous_frame[330:520, 20:w-20]
    previous_gray = cv2.cvtColor(previous_frame, cv2.COLOR_BGR2GRAY)

    transforms = np.zeros((total_frame_count - 1, 3), np.float32)

    current_idx = 0
    while (current_idx < total_frame_count - 1):
        print("Smoothing Status: " + str(current_idx / total_frame_count))

        previous_points = cv2.goodFeaturesToTrack(previous_gray, maxCorners=50, qualityLevel=0.01, minDistance=30, blockSize=3)

        is_success, current_frame = cap.read()
        current_frame = current_frame[330:520, 20:w-20]

        if not is_success:
            break

        try:
            current_gray = cv2.cvtColor(current_frame, cv2.COLOR_BGR2GRAY)

            current_points, status, error = cv2.calcOpticalFlowPyrLK(previous_gray, current_gray, previous_points, None)

            # assert previous_points.shape == current_points.shape

            idx = np.where(status == 1)[0]
            previous_points = previous_points[idx]
            current_points = current_points[idx]

            m = cv2.estimateAffine2D(previous_points, current_points)

            if m[0] is not None:
                dx = m[0][0, 2]
                dy = m[0][1, 2]
                da = np.arctan2(m[0][1, 0], m[0][0, 0])

                transforms[current_idx] = [dx, dy, da]
        except:
            print("Error")

        previous_gray = current_gray
        current_idx += 1

    # Compute trajectory using cumulative sums
    trajectory = np.cumsum(transforms, axis=0)
    smooth_trajectory = smooth(trajectory)

    difference = smooth_trajectory - trajectory
    transforms_smooth = transforms + difference

    cap.set(cv2.CAP_PROP_POS_FRAMES, 0)
    return transforms_smooth


def get_current_frame(frame, frame_idx, transforms_smooth, w, h):
    frame = frame[310:540, 20:w-20]

    dx = transforms_smooth[frame_idx, 0]
    dy = transforms_smooth[frame_idx, 1]
    da = transforms_smooth[frame_idx, 2]

    m = np.zeros((2, 3), np.float32)
    m[0, 0] = np.cos(da)
    m[0, 1] = -np.sin(da)
    m[1, 0] = np.sin(da)
    m[1, 1] = np.cos(da)
    m[0, 2] = dx
    m[1, 2] = dy

    frame_stabilized = cv2.warpAffine(frame, m, (w, h))

    return fixBorder(frame_stabilized)[20:210, 0:]


def save_stabilization_trajectories(transforms_smooth):
    with open("cache_video_stabilization_trajectories.txt", 'w') as file:
        json.dump(transforms_smooth.tolist(), file)


def read_stabilization_trajectories():
    with open("cache_video_stabilization_trajectories.txt", 'r') as file:
        return np.array(json.load(file))

