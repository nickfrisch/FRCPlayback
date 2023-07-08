import cv2
import matplotlib.pyplot as plt
import numpy as np

img = cv2.imread('images/match_pic2.png')[580:890, 0:]
gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
#gray = cv2.GaussianBlur(src=gray, ksize=(11,11), sigmaX=0)

cv2.imshow('Gray', gray)
cv2.waitKey(0)

dst = cv2.cornerHarris(gray, 4, 11, 0.2)
# dst = cv2.dilate(dst, None)

cv2.imshow('dst', dst)
cv2.waitKey(0)

img[dst > 0.01 * dst.max()] = [0, 0, 255]

cv2.imshow('img', img)
cv2.waitKey(0)
#plt.figure(figsize = [10, 10])
#plt.axis('off')
#plt.imshow(img[:,:,::-1])

#plt.show()