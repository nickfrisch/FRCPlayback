import cv2
import numpy as np

lowerRed = np.array((173, 52, 0), dtype = "uint8")
upperRed = np.array((180, 255, 255), dtype = "uint8")

#lowerRed = np.array((349, 94, 25), dtype = "uint8")
#upperRed = np.array((349, 80, 75), dtype = "uint8")

image = cv2.imread('images/match_pic2.png')
hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)

mask = cv2.inRange(hsv,lowerRed, upperRed)

res = cv2.bitwise_and(image,image, mask= mask)

#contours,hierarchy = cv2.findContours(res, 1, 2)

cv2.imshow("Before contouring", mask)
cv2.waitKey()

#res = cv2.drawContours(res, contours, -1, (255, 255, 255), 2)
#cv2.imshow("Contours", res)
#cv2.waitKey()