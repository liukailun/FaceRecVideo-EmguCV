using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using FaceSdk;
using System.IO;


namespace EmguCamTest2
{
    public partial class Form1 : Form
    {

        private Capture _capture = null;

        string ModelFileJDA = "Models\\ProductCascadeJDA27ptsWithLbf.mdl";
        string ModelFileCnnRecognition = "Models\\ProductRecognitionGoogleNetBNCnn27pts.mdl";

        FaceSdk.FaceDetectionJDA detector;
        FaceSdk.FaceRecognitionCNN recognizer;

        FaceSdk.Image grayImage;

        FaceSdk.Image MyImage1 = FaceSdk.ImageUtility.LoadImageFromFileAsRgb24("lkl.jpg");
        FaceSdk.Image MyGrayImage1 = FaceSdk.ImageUtility.LoadImageFromFileAsGray("lkl.jpg");
        FaceSdk.FaceFeature myFeature1;

        FaceSdk.Image MyImage2 = FaceSdk.ImageUtility.LoadImageFromFileAsRgb24("lyh.jpg");
        FaceSdk.Image MyGrayImage2 = FaceSdk.ImageUtility.LoadImageFromFileAsGray("lyh.jpg");
        FaceSdk.FaceFeature myFeature2;

        List<PictureBox> faceHistory = new List<PictureBox>();
        List<Label> SimilarityHistory = new List<Label>();
        List<PictureBox> faceHistory2 = new List<PictureBox>();

        int recResult = -1;

        System.Drawing.Image lkl = System.Drawing.Image.FromFile("lkl.jpg");
        System.Drawing.Image lyh = System.Drawing.Image.FromFile("lyh.jpg");
        System.Drawing.Image noface = System.Drawing.Image.FromFile("noface.jpg");

        Bitmap img1;
        Bitmap img2;
        Bitmap img3;



        public Form1()
        {
            InitializeComponent();

            detector = new FaceDetectionJDA(new Model(ModelFileJDA));
            recognizer = new FaceRecognitionCNN(new Model(ModelFileCnnRecognition));

            if (!MyImage1.IsEmpty() && !MyImage2.IsEmpty())
            {
                var myFRrecL1 = detector.DetectAndAlign(MyGrayImage1);
                var myFRrecL2 = detector.DetectAndAlign(MyGrayImage2);
                myFeature1 = recognizer.ExtractFaceFeature(MyImage1, myFRrecL1[0].Landmarks);
                myFeature2 = recognizer.ExtractFaceFeature(MyImage2, myFRrecL2[0].Landmarks);//这个地方出过错 myFRrecL2[0]写成了myFRrecL1[0] 我说咋一直只有我的照片
            }

            img1 = new Bitmap(lkl, 100, 100);
            img2 = new Bitmap(lyh, 100, 100);
            img3 = new Bitmap(noface, 100, 100);

            pictureBox1.Image = img1;
            pictureBox2.Image = img2;





            try
            {
                _capture = new Capture();
                _capture.ImageGrabbed += ProcessFrame;
                //timer1.Tick += new EventHandler(processFaces);
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            _capture.Start();
        }

        private void AddFaces(Mat frameToAdd, FaceSdk.FaceRectLandmarks[] faces)
        {

            PictureBox pic;
            Label simi;
            PictureBox pic2;

            //foreach (FaceSdk.Rectangle face in faces)
            //{
            //    pic = new PictureBox();
            //    //if (pic.Image != null)
            //    //{
            //    //    pic.Image.Dispose();
            //    //    pic.Image = null;
            //    //}


            //    System.Drawing.Rectangle FaceRect = new System.Drawing.Rectangle(face.Left, face.Top, face.Width, face.Height);
            //    //pic.Image = currentImage.Copy(FaceRect).ToBitmap();//这里应该把currentImage和帧frame联系起来
            //    Mat facePic = new Mat(frameToAdd, System.Drawing.Rectangle.Intersect(new System.Drawing.Rectangle(0, 0, frameToAdd.Cols, frameToAdd.Rows), FaceRect));
            //    Emgu.CV.CvInvoke.Resize(facePic, facePic, new Size(100, 100));
            //    pic.Image = facePic.Bitmap;
            //    pic.Width = pic.Image.Width;
            //    pic.Height = pic.Image.Height;

            //    faceHistory.Add(pic);

            //}

            foreach (FaceSdk.FaceRectLandmarks face in faces)
            {
                pic = new PictureBox();
                simi = new Label();
                pic2 = new PictureBox();
                //if (pic.Image != null)
                //{
                //    pic.Image.Dispose();
                //    pic.Image = null;
                //}

                System.Drawing.Rectangle FaceRect = new System.Drawing.Rectangle(face.FaceRect.Left, face.FaceRect.Top, face.FaceRect.Width, face.FaceRect.Height);


                //pic.Image = currentImage.Copy(FaceRect).ToBitmap();//这里应该把currentImage和帧frame联系起来
                Mat facePic = new Mat(frameToAdd, System.Drawing.Rectangle.Intersect(new System.Drawing.Rectangle(0, 0, frameToAdd.Cols, frameToAdd.Rows), FaceRect));




                FaceSdk.Image Image2313 = ImageUtility.LoadImageFromBitmapAsRgb24(frameToAdd.Bitmap);

                //这里出错 应该直接用mat 而不是部分图像！
                FaceSdk.FaceFeature feature = recognizer.ExtractFaceFeature(Image2313, face.Landmarks);
                float simi1 = -1;
                float simi2 = -1;
                if (myFeature1 != null && myFeature2 != null)
                {
                    simi1 = 1 - (recognizer.GetFaceFeatureDistance(myFeature1, feature));
                    simi2 = 1 - (recognizer.GetFaceFeatureDistance(myFeature2, feature));



                    if (simi1 > simi2 && simi1 > 0.5)
                    {
                        recResult = 1;
                    }
                    else if (simi2 > simi1 && simi2 > 0.5)
                    {
                        recResult = 2;
                    }
                    else if (simi1 < 0.5 && simi2 < 0.5)
                    {
                        recResult = 0;
                    };
                }




                //Action method1 = (Action)delegate
                //{

                //    label1.Text = (1 - result).ToString();

                //};

                //if (method1 != null)
                //{
                //    flowLayoutPanel1.Invoke(method1);
                //}

                //

                //DateTime time=new DateTime();
                string fileName = DateTime.Now.ToFileTimeUtc().ToString();
                //string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");
                if (!Directory.Exists("OutputImage"))
                {
                    Directory.CreateDirectory("OutputImage");
                }



                if (checkBox1.Checked == true)
                {
                    facePic.Save("OutputImage\\" + fileName + ".jpg");
                }







                //simi.Text = (1 - simi1).ToString();

                if (recResult == 1)
                {
                    simi.Text = simi1.ToString() + " lkl";
                    pic2.Image = img1;
                }
                else if (recResult == 2)
                {
                    simi.Text = simi2.ToString() + " lyh";
                    pic2.Image = img2;
                }
                else if (recResult == 0)
                {
                    simi.Text = "no this person";
                    pic2.Image = img3;
                }









                Emgu.CV.CvInvoke.Resize(facePic, facePic, new Size(100, 100));
                pic.Image = facePic.Bitmap;
                pic.Width = pic.Image.Width;
                pic.Height = pic.Image.Height;


                //在这里给pic.Image加相似度
                simi.Height = pic.Image.Height;
                pic2.Height = pic.Image.Height;

                SimilarityHistory.Add(simi);
                faceHistory.Add(pic);
                faceHistory2.Add(pic2);

            }


            //flowLayoutPanel1.Controls.AddRange(faceHistory.ToArray());//多线程问题 就是在这出现的，我觉得上面那一行也有问题 但是报错是在这一行出现的


            Action method = (Action)delegate
            {

                flowLayoutPanel1.Controls.Clear();

                flowLayoutPanel1.Controls.AddRange(faceHistory.ToArray());//这句话好像真的没什么用，下个程序中就不要了(FaceRecCam)

                //if (faceHistory.Count > 0)
                //    flowLayoutPanel1.ScrollControlIntoView(faceHistory[faceHistory.Count - 1]);



                flowLayoutPanel2.Controls.Clear();

                flowLayoutPanel2.Controls.AddRange(SimilarityHistory.ToArray());

                //if (SimilarityHistory.Count > 0)
                //{
                //    flowLayoutPanel2.ScrollControlIntoView(SimilarityHistory[SimilarityHistory.Count - 1]);
                //}
                flowLayoutPanel3.Controls.Clear();

                flowLayoutPanel3.Controls.AddRange(faceHistory2.ToArray());

            };

            if (method != null)
            {
                flowLayoutPanel1.Invoke(method);
            }


            faceHistory.Clear();//加了这句就不报错说对象被使用了 但是会卡死，删除了 AddFace函数开头的flowLayoutPanel1.Controls.Clear();解决了卡顿问题
            SimilarityHistory.Clear();
            faceHistory2.Clear();
        }


        //

        private void ProcessFrame(object sender, EventArgs e)
        {

            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);
            //FaceSdk.Rectangle[] detAlnRst;
            FaceSdk.FaceRectLandmarks[] decResult;



            grayImage = FaceSdk.ImageUtility.LoadImageFromBitmapAsGray(frame.Bitmap);
            //detAlnRst = detector.Detect(grayImage);
            decResult = detector.DetectAndAlign(grayImage);

            //foreach (FaceSdk.Rectangle i in detAlnRst)
            //{
            //    System.Drawing.Rectangle j = new System.Drawing.Rectangle(i.Left, i.Top, i.Width, i.Height);
            //    CvInvoke.Rectangle(frame, j, new MCvScalar(0, 255.0, 0), 2);
            //}

            AddFaces(frame, decResult);


            foreach (FaceSdk.FaceRectLandmarks i in decResult)
            {
                System.Drawing.Rectangle j = new System.Drawing.Rectangle(i.FaceRect.Left, i.FaceRect.Top, i.FaceRect.Width, i.FaceRect.Height);
                CvInvoke.Rectangle(frame, j, new MCvScalar(0, 255.0, 0), 2);
            }

            imageBox1.Image = frame;

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private float Compare(Bitmap bitmap1, Bitmap bitmap2)//比较人脸的函数，挺耗时的，我测了一下 两张250*250的比较 需要150-180ms 耗时主要在抽特征 而不是计算距离
        {

            FaceSdk.Image FacesdkImage1 = FaceSdk.ImageUtility.LoadImageFromBitmapAsRgb24(bitmap1);
            FaceSdk.Image FacesdkImage2 = FaceSdk.ImageUtility.LoadImageFromBitmapAsRgb24(bitmap2);

            FaceSdk.Image grayImage1 = ImageUtility.LoadImageFromBitmapAsGray(bitmap1);
            FaceSdk.Image grayImage2 = ImageUtility.LoadImageFromBitmapAsGray(bitmap2);

            var detAlnRst1 = detector.DetectAndAlign(grayImage1);
            var detAlnRst2 = detector.DetectAndAlign(grayImage2);

            if (detAlnRst1.Count() > 0 && detAlnRst2.Count() > 0)
            {

                var feature1 = recognizer.ExtractFaceFeature(FacesdkImage1, detAlnRst1[0].Landmarks);
                var feature2 = recognizer.ExtractFaceFeature(FacesdkImage2, detAlnRst2[0].Landmarks);
                float result = recognizer.GetFaceFeatureDistance(feature1, feature2);

                return result;
            }
            return -1;
        }
    }
}
