#include <opencv2/opencv.hpp>
#include <iostream>

//using namespace cv;
using namespace std;

int main(int argc, char** argv)
{
    // Read the image file
    cv::Mat image = cv::imread("C:\\Users\Wyatt\Downloads\Batch1\image.jpeg");
    // Check for failure
    if (image.empty())
    {
        cout << "Image Not Found!!!" << endl;
        //cin.get(); //wait for any key press
        //return -1;
    }
    // Show our image inside a window.
    //cv::imshow("Image Window Name here", image);

    // Wait for any keystroke in the window
    cv::waitKey(0);
    return 0;
}