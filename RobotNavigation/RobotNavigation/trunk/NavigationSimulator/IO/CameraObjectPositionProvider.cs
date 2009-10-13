using System;
using System.Collections.Generic;
using System.Text;
using MMCar_Finder;
using System.Drawing;
using System.Windows.Forms;
using MarkerFinderTest;
using NavigationSimulator;
using System.Threading;

namespace OnlabNeuralis
{
   
    public class CameraObjectPositionProvider : ICarPositionProvider, IObstaclePositionProvider, IFinishPositionProvider, ISampler
    {
        CameraDirectShow camera;
        MyMarkerFinder mf;                
        PictureBox pb;
        private const int COUNT_MAX = 1;
        
        
        FinishState currentFinishState;
        CarModelState currentCarModelState;
        CarModelState sampledCarState;
        
        PositionAndOrientationPredictor finishPredictor;
        List<ObstacleModel> obstacles;
        List<ObstacleState> currentObstacleStates;

        Thread trainThread;
        bool running;


        public CameraObjectPositionProvider(PictureBox pb)
        {
            this.pb = pb;
            camera = new CameraDirectShow();
            camera.OnNewFrame += new OnNewFrameDelegate(camera_OnNewFrame);
            camera.Start();
            finishPredictor = new PositionAndOrientationPredictor(30, 10);
            obstacles = new List<ObstacleModel>();
            currentCarModelState = new CarModelState(new PointD(ComMath.Normal(0.05, 0, 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                             ComMath.Normal(0.05, 0, 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                  new PointD(0, 1));
            currentFinishState = new FinishState(new PointD(ComMath.Normal(0.95, 0, 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                ComMath.Normal(0.95, 0, 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                     0.5 * Math.PI);
        }

        void camera_OnNewFrame(Bitmap frame)
        {
            if (pb != null) pb.Image = frame;
        }

        public void StartMarkerFinding(Image background, Image carMarker, Image finishMarker)         
        {
            List<Bitmap> markerlist = new List<Bitmap>();
            markerlist.Add(new Bitmap(carMarker));
            markerlist.Add(new Bitmap(finishMarker));
            mf = new MyMarkerFinder(markerlist, new Bitmap(background));          
        }

        public void StartTrain()
        {
            trainThread = new Thread(new ThreadStart(run));
            running = true;
            trainThread.IsBackground = true;
            trainThread.Start();
        }

        public void run()
        {
            while (running)
            {
                finishPredictor.Train();
                lock (obstacles)
                {
                    foreach (ObstacleModel ops in obstacles)
                    {
                        lock (ops)
                        {
                            ops.state.pp.Train();
                        }
                    }
                }
                Thread.Sleep(0);
            }
        }

        public void StopTrain()
        {
            running = false;
        }

        public Image GetImage()
        {
            return camera.GetBitmap();
        }




        public void Simulate(IModelSimulator modelsim, CarModelInput input, double timestep)
        {
            CarModel model = new CarModel(GetCarState());
            model.SimulateModel(input, modelsim, timestep);
            currentCarModelState = model.state;
        }
      
        public CarModelState GetCarState()
        {
            return sampledCarState;
        }

        public CarModel GetCarModel()
        {
            return new CarModel(currentCarModelState);
        }


        public List<ObstacleState> GetObstacleStates(int iteration)
        {
            if (iteration > 0)
            {
                List<ObstacleState> oss = new List<ObstacleState>();
                lock (obstacles)
                {
                    foreach (ObstacleModel om in obstacles)
                    {
                        lock (om)
                        {
                            List<PointD> p = om.state.pp.PredictNextPositions(iteration);
                            if (p != null) oss.Add(new ObstacleState(p[iteration - 1], om.state.radius));
                            else oss.Add(new ObstacleState(om.state.pp.position, om.state.radius));
                        }
                    }
                }
                return oss;
            }
            else
            {
                List<ObstacleState> ret = new List<ObstacleState>();
                lock (obstacles)
                {
                    foreach (ObstacleModel om in obstacles)
                    {
                        lock (om)
                        {
                            ret.Add(om.state);
                        }
                    }
                }
                return ret;
            }
        }

        public List<ObstacleModel> GetObstacleModels(int iteration)
        {
            if (iteration > 0)
            {
                List<ObstacleState> states = GetObstacleStates(iteration);
                List<ObstacleModel> list = new List<ObstacleModel>();
                if (states != null)
                {
                    foreach (ObstacleState os in states)
                    {
                        list.Add( new ObstacleModel(os.pp.position, os.radius));
                    }
                }
                return list;
            }
            else
            {
                return obstacles;
            }
        }   


        public FinishState GetFinishState(int iteration)
        {
            if (iteration > 0)
            {

                List<PointD> p = finishPredictor.PredictNextPositions(iteration);
                List<PointD> o = finishPredictor.PredictNextOrientations(iteration);
                if ((p != null) && (o != null)) return new FinishState(p[iteration - 1], o[iteration - 1]);
                else return currentFinishState;
            }
            else
            {
                return currentFinishState;
            }
        }

        public FinishModel GetFinishModel(int iteration)
        {
            FinishModel fm = new FinishModel(GetFinishState(iteration));
            fm.SetSelectedState(1, 0);
            return fm;
        }

        //public CarModelState GetState(int index)
        //{
        //    double[] ret = GetPosition(index);
        //    CarModelState cs = new CarModelState();
        //    cs.Angle = -ret[2];
        //    //cs.Position = new PointD(ComMath.Normal(ret[0], 0, im.Width, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X), ComMath.Normal(im.Height - ret[1], 0, im.Height, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y));
        //    return cs;
        //}



        public void TakeSample()
        {
            RunMarkerFinder();
            sampledCarState = currentCarModelState;
        }

        private void RunMarkerFinder()
        {
            if (mf != null)
            {
                Bitmap frame = camera.GetBitmap();
                List<Shape> shapes = null;

                shapes = mf.ProcessFrame(frame);

                bool carFound = false;
                bool finishFound = false;
                
               // finish
                currentObstacleStates = new List<ObstacleState>();
                foreach (Shape s in shapes)
                {
                    if ((s.index == 0) && (s.scale > 0.17) && (s.scale < 0.22))
                    {
                        currentCarModelState = new CarModelState(new PointD(ComMath.Normal(s.pos.X, 0, frame.Width, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                                            ComMath.Normal(s.pos.Y, 0, frame.Height, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                                 -Math.PI / 2 - s.rot);
                        carFound = true;
                    }
                    else if ((s.index == 1) && (s.scale > 0.16) && (s.scale < 0.20))
                    {
                        currentFinishState = new FinishState(new PointD(ComMath.Normal(s.pos.X, 0, frame.Width, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                                        ComMath.Normal(s.pos.Y, 0, frame.Height, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                                 -Math.PI / 2 - s.rot);
                        finishPredictor.AddPoint(currentFinishState.Position, currentFinishState.Orientation);
                        finishFound = true;
                    }
                    else
                    {
                        currentObstacleStates.Add(new ObstacleState(new PointD(ComMath.Normal(s.pos.X, 0, frame.Width, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                                               ComMath.Normal(s.pos.Y, 0, frame.Height, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                                 s.scale * 500));
                        //ide kell egy robusztus alg
                        //ha zaj jon, akkor ne vegye be
                    }
                }

                lock (obstacles)
                {
                    List<ObstacleState> curObState = new List<ObstacleState>(currentObstacleStates);
                    foreach (ObstacleModel ops in obstacles)
                    {
                        List<PointD> list = ops.state.pp.PredictNextPositions(1);
                        ObstacleState osw =null;

                        PointD pd;
                        if (list != null) pd = list[0];
                        else pd = ops.state.pp.position;

                        double mindist = double.MaxValue;
                        foreach (ObstacleState os in curObState)
                        {
                            double dist = (os.pp.position.X - pd.X) * (os.pp.position.X - pd.X) + (os.pp.position.Y - pd.Y) * (os.pp.position.Y - pd.Y); 
                            if (dist < mindist) 
                            {
                                mindist = dist;
                                osw = os;
                            }
                        }

                        if (osw != null)
                        {
                            lock (ops)
                            {
                                ops.state.pp.AddNewPosition(osw.pp.position);
                            }
                            curObState.Remove(osw);
                        }
                        else
                        {
                            //a predikcioval leptetjuk tovabb
                            lock (ops)
                            {
                                ops.state.pp.AddNewPosition(pd);
                            }
                        }
                    }

                    foreach(ObstacleState os in curObState) 
                    {
                        lock (obstacles)
                        {
                            ObstacleModel om = new ObstacleModel(os.pp.position, os.radius);
                            om.SetSelectedState(1, 0);
                            if (obstacles.Count < 1) obstacles.Add(om);
                        }
                    }


                }
            }
        }
    }
}
