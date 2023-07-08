import numpy as np
import cv2
import matplotlib.pyplot as plt

def get_destination_points(corners):
    w1 = np.sqrt((corners[0][0] - corners[1][0]) ** 2 + (corners[0][1] - corners[1][1]) ** 2)
    w2 = np.sqrt((corners[2][0] - corners[3][0]) ** 2 + (corners[2][1] - corners[3][1]) ** 2)
    w = max(int(w1), int(w2))

    h1 = np.sqrt((corners[0][0] - corners[2][0]) ** 2 + (corners[0][1] - corners[2][1]) ** 2)
    h2 = np.sqrt((corners[1][0] - corners[3][0]) ** 2 + (corners[1][1] - corners[3][1]) ** 2)
    h = max(int(h1), int(h2))

    w_add = 500
    h_add = 500

    destination_corners = np.float32([(0 + w_add, 0 + h_add), (w + w_add - 1, 0 + h_add), (0 + w_add, h - 1 + h_add), (w + w_add - 1, h - 1 + h_add)])

    print('\nThe destination points are: \n')
    for index, c in enumerate(destination_corners):
        character = chr(65 + index) + "'"
        print(character, ':', c)

    return destination_corners, h, w


image = cv2.imread('images/match_pic2.png')

cv2.namedWindow('win')

corners = ((608, 694), (1254, 684), (517, 750), (1247, 746))
cv2.rectangle(image, corners[0], corners[3], (0, 255, 0), thickness=2)
cv2.imshow('win', image)
cv2.waitKey(0)

#destination, h, w = get_destination_points(corners)
H, _ = cv2.findHomography(np.float32(corners), destination, method=cv2.RANSAC, ransacReprojThreshold=3.0)
print('\nThe homography matrix is: \n', H)

# Backproject from DST to SRC
[xw, yw, w] = np.dot(H, np.array([960, 777, 1]))
result = [xw / w, yw / w]
print('\nResulting Values are: \n', result[0], ', ', result[1])

unwarped_top_left = np.dot(H, np.array([corners[0][0], corners[0][1], 1]))
unwarped_top_left = [unwarped_top_left[0] / unwarped_top_left[2], unwarped_top_left[1] / unwarped_top_left[2]]
print('\nResulting Left Top Values are: \n', unwarped_top_left)
unwarped_top_right = np.dot(H, np.array([corners[1][0], corners[1][1], 1]))
unwarped_top_right = [unwarped_top_right[0] / unwarped_top_right[2], unwarped_top_right[1] / unwarped_top_right[2]]
print('Resulting Right Top Values are: \n', unwarped_top_right)
unwarped_bottom_left = np.dot(H, np.array([corners[2][0], corners[2][1], 1]))
unwarped_bottom_left = [unwarped_bottom_left[0] / unwarped_bottom_left[2], unwarped_bottom_left[1] / unwarped_bottom_left[2]]
print('Resulting Left Bottom Values are: \n', unwarped_bottom_left)
unwarped_bottom_right = np.dot(H, np.array([corners[3][0], corners[3][1], 1]))
unwarped_bottom_right = [unwarped_bottom_right[0] / unwarped_bottom_right[2], unwarped_bottom_right[1] / unwarped_bottom_right[2]]
print('Resulting Right Bottom Values are: \n', unwarped_bottom_right)

xScalar = (result[0] - unwarped_top_left[0]) / (unwarped_top_right[0] - unwarped_top_left[0])
yScalar = (result[1] - unwarped_top_left[1]) / (unwarped_bottom_left[1] - unwarped_top_left[1])

print("\nX Scalar: ", xScalar)
print("Y Scalar: ", yScalar)

field_corners = ((350, 135), (781, 134), (351, 258))
field_x = field_corners[0][0] + (xScalar * (field_corners[1][0] - field_corners[0][0]))
field_y = field_corners[0][1] + (yScalar * (field_corners[2][1] - field_corners[0][1]))

print("\nX Field: ", field_x)
print("Y Field: ", field_y)

field_img = cv2.imread('images/2023-field-flipped.png')
field_img = cv2.circle(field_img, (int(field_x), int(field_y)), radius=5, color=(0, 0, 255), thickness=-1)
cv2.imshow('win', field_img)
cv2.waitKey(0)

# cv2.rectangle(image, corners[0], corners[3], (0, 255, 0), thickness=2)
# cv2.imshow('Result', image)
# cv2.waitKey(0)

"""
pts1 = np.float32([[56,65],[368,52],[28,387],[389,390]])
pts2 = np.float32([[0,0],[300,0],[0,300],[300,300]])
M = cv.getPerspectiveTransform(pts1,pts2)
dst = cv.warpPerspective(img,M,(300,300))
"""