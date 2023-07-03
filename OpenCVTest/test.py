import cv2
import numpy as np
import matplotlib.pyplot as plt

__filter_contours_min_area = 2700.0
__filter_contours_min_perimeter = 0.0
__filter_contours_min_width = 100.0
__filter_contours_max_width = 150.0
__filter_contours_min_height = 20.0
__filter_contours_max_height = 100.0
#__filter_contours_solidity = [0.0, 62.121212121212125]
__filter_contours_solidity = [0.0, 100]
__filter_contours_max_vertices = 1000000
__filter_contours_min_vertices = 0
__filter_contours_min_ratio = 0
__filter_contours_max_ratio = 1000

def get_destination_points(corners):
    w1 = np.sqrt((corners[0][0] - corners[1][0]) ** 2 + (corners[0][1] - corners[1][1]) ** 2)
    w2 = np.sqrt((corners[2][0] - corners[3][0]) ** 2 + (corners[2][1] - corners[3][1]) ** 2)
    w = max(int(w1), int(w2))

    h1 = np.sqrt((corners[0][0] - corners[2][0]) ** 2 + (corners[0][1] - corners[2][1]) ** 2)
    h2 = np.sqrt((corners[1][0] - corners[3][0]) ** 2 + (corners[1][1] - corners[3][1]) ** 2)
    h = max(int(h1), int(h2))

    w_add = 00
    h_add = 00

    destination_corners = np.float32([(0 + w_add, 0 + h_add), (w + w_add - 1, 0 + h_add), (0 + w_add, h - 1 + h_add), (w + w_add - 1, h - 1 + h_add)])

    print('\nThe destination points are: \n')
    for index, c in enumerate(destination_corners):
        character = chr(65 + index) + "'"
        print(character, ':', c)

    print('\nThe approximated height and width of the original image is: \n', (h, w))
    return destination_corners, h, w


def unwarp(img, src, dst):
    h, w = img.shape[:2]
    H, _ = cv2.findHomography(src, dst, method=cv2.RANSAC, ransacReprojThreshold=3.0)
    print('\nThe homography matrix is: \n', H)
    un_warped = cv2.warpPerspective(img, H, (w, h), flags=cv2.INTER_LINEAR#|cv2.WARP_INVERSE_MAP
                                    )
    #warpTwoImages(img, H, w, h)

    # plot

    f, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 8))
    # f.subplots_adjust(hspace=.2, wspace=.05)
    ax1.imshow(img)
    ax1.set_title('Original Image')

    x = [src[0][0], src[2][0], src[3][0], src[1][0], src[0][0]]
    y = [src[0][1], src[2][1], src[3][1], src[1][1], src[0][1]]

    ax2.imshow(img)
    ax2.plot(x, y, color='yellow', linewidth=3)
    ax2.set_ylim([h, 0])
    ax2.set_xlim([0, w])
    ax2.set_title('Target Area')

    plt.show()
    return un_warped

def __filter_contours(input_contours, min_area, min_perimeter, min_width, max_width,
                        min_height, max_height, solidity, max_vertex_count, min_vertex_count,
                        min_ratio, max_ratio):
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
            print(area)
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

def example_one():
    image = cv2.imread('images/match_pic2.png')
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

    #corners = ((501, 666), (1414, 646), (291, 790), (1432, 793))
    corners = ((373, 666), (1608, 640), (86, 777), (1727, 779))
    destination, h, w = get_destination_points(corners)
    un_warped = unwarp(image, np.float32(corners), destination)

    cropped = un_warped[int(destination[0][1]):int(destination[2][1]), int(destination[0][0]):int(destination[1][0])]

    f, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 5), facecolor='w', edgecolor='k')
    # f.subplots_adjust(hspace=.2, wspace=.05)

    lowerRed = np.array((173, 52, 0), dtype = "uint8")
    upperRed = np.array((180, 255, 255), dtype = "uint8")

    #hsv = cv2.cvtColor(un_warped, cv2.COLOR_RGB2HSV)
    hsv = cv2.cvtColor(cropped, cv2.COLOR_RGB2HSV)

    mask = cv2.inRange(hsv,lowerRed, upperRed)
    contours,hierarchy = cv2.findContours(mask, 1, 2)
    contours = __filter_contours(contours, __filter_contours_min_area, __filter_contours_min_perimeter, __filter_contours_min_width, __filter_contours_max_width, __filter_contours_min_height, __filter_contours_max_height, __filter_contours_solidity, __filter_contours_max_vertices, __filter_contours_min_vertices, __filter_contours_min_ratio, __filter_contours_max_ratio)
    #res = cv2.bitwise_and(image,image, mask= mask)
    res = cv2.drawContours(cropped, contours, -1, (0, 255, 0), 5)

    x_rect,y_rect,w_rect,h_rect = cv2.boundingRect(contours[0])
    src = ((x_rect, y_rect),(x_rect+w_rect,y_rect),(x_rect,y_rect+h_rect),(x_rect+w_rect,y_rect+h_rect))

    res = cv2.rectangle(res, (src[0][0], src[0][1]),(src[3][0],src[3][1]), (255,0,0), 2)

    ax1.imshow(un_warped)
    ax2.imshow(cropped)

    plt.show()

    f, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 8))

    img = cv2.imread("images/2023-field.png")

    #size_of_original_x, size_of_original_y = image.shape[:2]
    size_of_cropped_x, size_of_cropped_y = cropped.shape[:2]
    size_of_field_x, size_of_field_y = img.shape[:2]

    conversion_x = size_of_cropped_x / size_of_field_x
    # conversion_y = size_of_cropped_y / size_of_field_y

    # conversion_x = size_of_field_x / size_of_cropped_x
    # conversion_y = size_of_field_y / size_of_cropped_y

    # conversion_x = 1
    conversion_y = 1

    x = [src[0][0] * conversion_x, src[2][0] * conversion_x, src[3][0] * conversion_x, src[1][0] * conversion_x, src[0][0] * conversion_x]
    y = [src[0][1] * conversion_y, src[2][1] * conversion_y, src[3][1] * conversion_y, src[1][1] * conversion_y, src[0][1] * conversion_y]

    img = cv2.flip(img, 0)
    #img = cv2.flip(img, 1)

    ax1.imshow(img)
    ax1.plot(x, y, color='red', linewidth=3)
    ax1.set_ylim([h*3, 0])
    ax1.set_xlim([0, w])
    ax2.imshow(cropped)

    plt.show()

if __name__ == '__main__':
    example_one()