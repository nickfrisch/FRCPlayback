import cv2
import numpy as np
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

def example_one():
    image = cv2.imread('images/match_pic2.png')
    image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    #plt.imshow(image)
    #plt.title('Original Image')
    #plt.show()

    #corners = shi_tomashi(image)
    corners = ((501, 666), (1414, 646), (291, 790), (1432, 793))
    destination, h, w = get_destination_points(corners)
    un_warped = unwarp(image, np.float32(corners), destination)
    #cropped = un_warped[400:h+1300, 0:w+500+400]
    cropped = un_warped[0:h, 0:w]

    f, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 5), facecolor='w', edgecolor='k')
    # f.subplots_adjust(hspace=.2, wspace=.05)

    ax1.imshow(un_warped)
    ax2.imshow(cropped)

    plt.show()

    f, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 8))

    img = cv2.imread("images/2023-field.png")

    #size_of_original_x, size_of_original_y = image.shape[:2]
    size_of_cropped_x, size_of_cropped_y = cropped.shape[:2]
    size_of_field_x, size_of_field_y = img.shape[:2]

    conversion_x = size_of_cropped_x / size_of_field_x
    conversion_y = size_of_cropped_y / size_of_field_y

    src = ((1203, 290),(1308,290),(1220,319),(1313,313))

    x = [src[0][0] * conversion_x, src[2][0] * conversion_x, src[3][0] * conversion_x, src[1][0] * conversion_x, src[0][0] * conversion_x]
    y = [src[0][1] * conversion_y, src[2][1] * conversion_y, src[3][1] * conversion_y, src[1][1] * conversion_y, src[0][1] * conversion_y]

    img = cv2.flip(img, 0)
    img = cv2.flip(img, 1)

    ax1.imshow(img)
    ax1.plot(x, y, color='red', linewidth=3)
    ax1.set_ylim([h*3, 0])
    ax1.set_xlim([0, w])
    ax2.imshow(un_warped)

    plt.show()

if __name__ == '__main__':
    example_one()