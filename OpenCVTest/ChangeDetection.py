import cv2
import numpy as np
import VideoStabilization

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

def classify_bounding_box(imgHSV, lowRed=(163, 66, 66), highRed=(180, 255, 255)):
    mask = cv2.inRange(imgHSV, lowRed, highRed)
    percent_valid_red = round(np.count_nonzero(mask) / np.size(mask) * 100, 1)

    return percent_valid_red

def motion_detection(stabilization=False, readTrajectoriesFromCache=False):
    frame_count = 0
    previous_frame = None

    vid = cv2.VideoCapture("C:\\Users\Wyatt\AppData\Local\Temp\FRCPlayback_video.mp4")
    total_frame_count = int(vid.get(cv2.CAP_PROP_FRAME_COUNT))

    w = int(vid.get(cv2.CAP_PROP_FRAME_WIDTH))
    h = int(vid.get(cv2.CAP_PROP_FRAME_HEIGHT))

    if stabilization:
        if not readTrajectoriesFromCache:
            transforms_smooth = VideoStabilization.preprocessing(vid, w, h, total_frame_count)
            VideoStabilization.save_stabilization_trajectories(transforms_smooth)
        else:
            transforms_smooth = VideoStabilization.read_stabilization_trajectories()


    for i in range(total_frame_count - 2):
        is_success, frame = vid.read()
        if not is_success:
            break

        if stabilization:
            frame = VideoStabilization.get_current_frame(frame, i, transforms_smooth, w, h)
        else:
            frame = frame[330:520, 20:w-20]

        frame_hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
        prepared_frame = cv2.cvtColor(frame, code=cv2.COLOR_BGR2GRAY)
        prepared_frame = cv2.blur(src=prepared_frame, ksize=(11,11))

        if (previous_frame is None):
            previous_frame = prepared_frame
            continue

        diff_frame = cv2.absdiff(src1=previous_frame, src2=prepared_frame)
        previous_frame = prepared_frame

        kernel = np.ones((13, 13))
        diff_frame = cv2.dilate(diff_frame, kernel, 1)
        #diff_frame = cv2.GaussianBlur(src=diff_frame, ksize=(7, 7), sigmaX=0)

        thresh_frame = cv2.threshold(src=diff_frame, thresh=10, maxval=200, type=cv2.THRESH_BINARY)[1]

        contours, _ = cv2.findContours(image=thresh_frame, mode=cv2.RETR_EXTERNAL, method=cv2.CHAIN_APPROX_SIMPLE)

        contours = filter_contours(contours, 150, 10)
        #cv2.drawContours(image=frame, contours=contours, contourIdx=-1, color=(0, 255, 0), thickness=2, lineType=cv2.LINE_AA)

        for c in contours:
            rec = cv2.boundingRect(c) # x, y, w, h
            if rec[2] * rec[3] > 1000:
                #cv2.imshow("test", cv2.inRange(frame_hsv, (173, 52, 0), (180, 255, 255)))
                #cv2.waitKey(0)
                percent = classify_bounding_box(frame_hsv[rec[1]:rec[1] + rec[3], rec[0]:rec[0] + rec[2]])
                if percent > 5:
                    cv2.rectangle(frame, rec, (0, 0, 255), thickness=2)
                else:
                    cv2.rectangle(frame, rec, (0, 255, 0), thickness=2)

                cv2.putText(frame, str(percent), (rec[0], rec[1] - 10), cv2.FONT_HERSHEY_PLAIN, 0.9, (0, 255, 0), 2)

        cv2.imshow('Motion Detector', frame)

        frame_count += 1

        #cv2.imshow('Motion Detector', frame)

        if cv2.waitKey(10) == 27:
            break

motion_detection(stabilization=True, readTrajectoriesFromCache=True)
