import cv2
import numpy as np

img = cv2.imread('images/match_pic2.png')[580:890, 0:]
gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
gray = cv2.GaussianBlur(src=gray, ksize=(5,5), sigmaX=0)

edges = cv2.Canny(gray, 150, 200, apertureSize = 3)
cv2.imshow('edges',edges)
cv2.waitKey(0)

minLineLength = 150
maxLineGap = 10
lines = cv2.HoughLinesP(edges, 1, np.pi/180, 30, minLineLength=minLineLength, maxLineGap=maxLineGap)

for x in range(0, len(lines)):
    for x1,y1,x2,y2 in lines[x]:
        #if abs((y1 - y2) / (x1 - x2)) > 1:
        cv2.line(img,(x1,y1),(x2,y2),(0,255,0),2)

cv2.imshow('hough',img)
cv2.waitKey(0)