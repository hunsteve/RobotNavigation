using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlabNeuralis;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace NavigationSimulator
{
    
    public class SimulationModeItemManager : ICarPositionProvider, IFinishPositionProvider, IObstaclePositionProvider, ISampler
    {
        enum State
        {
            NONE = 0,
            DRAGGEDINSIDE,
            DRAGGEDOUTSIDE,
            ERROR
        };

        

        private State state;

        List<ObstacleModel> obstacles;

        List<IDragable> dragables;
        IDragable dragged, inselected, outselected;

        private CarModel model;
        private FinishModel finish;
        private CarModelState sampledCarState;

        private CarModelGraphicControl graphicControl;
        private ContextMenuStrip contextMenuStrip;

        private Point contextMenuLocation;

        Thread trainThread;
        bool running;

        DateTime lastCtrlTime;
        Point lastCtrlLocation;

        public SimulationModeItemManager(ContextMenuStrip contextMenuStrip, CarModelGraphicControl graphicControl)
        {
            model = new CarModel(new CarModelState(new PointD(ComMath.Normal(0.3, 0, 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                             ComMath.Normal(0.3, 0, 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                                  new PointD(0, 1)));
            finish = new FinishModel(new PointD(ComMath.Normal(0.5, 0, 1, CarModelState.MIN_POS_X, CarModelState.MAX_POS_X),
                                                ComMath.Normal(0.5, 0, 1, CarModelState.MIN_POS_Y, CarModelState.MAX_POS_Y)),
                                     0.5 * Math.PI);
            obstacles = new List<ObstacleModel>();
            dragables = new List<IDragable>();

            sampledCarState = model.state;
            
            dragables.Add(model);
            dragables.Add(finish);
            dragged = null;

            state = State.NONE;

            this.contextMenuStrip = contextMenuStrip;
            this.graphicControl = graphicControl;

            contextMenuStrip.Items[0].Click += new EventHandler(newObstacle);
            contextMenuStrip.Items[1].Click += new EventHandler(deleteObstacle);
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
                double err;
                lock (obstacles)
                {
                    foreach (ObstacleModel ops in obstacles)
                    {
                        lock (ops)
                        {
                            err = ops.state.pp.Train();
                            Console.Out.WriteLine(err);
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

        public void SimualteGrid(IGridModelSimulator sim, GridCarModelInput input, double timestep)
        {
            GridCarModelState gcms;
            sim.SimulateModel(GridCarModelState.FromCarModelState(model.state), input, timestep, out gcms);
            model.state = GridCarModelState.ToCarModelState(gcms);
        }

        public void Simulate(IModelSimulator modelsim, CarModelInput input, double timestep)
        {
            model.SimulateModel(input, modelsim, timestep);
            lock (obstacles)
            {
                foreach (ObstacleModel ops in obstacles)
                {
                    lock (ops)
                    {
                        ops.state.pp.Simulate(timestep);
                    }
                }
            }
        }

        private IDragable getInsideDragabe(Point p)
        {
            int i = 0;
            while ((i < dragables.Count) && (!dragables[i].PointInInsideArea(p))) { ++i; };
            if (i >= dragables.Count) return null;
            else return dragables[i];
        }

        private IDragable getOutsideDragabe(Point p)
        {
            int i = 0;
            while ((i < dragables.Count) && (!dragables[i].PointInOutsideArea(p))) { ++i; };
            if (i >= dragables.Count) return null;
            else return dragables[i];
        }

        public bool MouseDown(MouseEventArgs e, Matrix transform)
        {
            Point[] pp = new Point[] { e.Location };
            transform.TransformPoints(pp);
            Point location = pp[0];

            if (e.Button == MouseButtons.Left)
            {
                if (state == State.NONE)
                {
                    IDragable inside = getInsideDragabe(location);
                    IDragable outside = getOutsideDragabe(location);
                    if (inside != null)
                    {
                        dragged = inside;
                        state = State.DRAGGEDINSIDE;
                        inselected.SetSelectedState(2, 0);                        
                        return true;
                    }
                    else if (outside != null)
                    {

                        dragged = outside;
                        state = State.DRAGGEDOUTSIDE;
                        outselected.SetSelectedState(0, 2);                        
                        return true;
                    }                    
                    return false;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                dragged = getInsideDragabe(location);
                if (dragged == null) dragged = getOutsideDragabe(location);
                if ((dragged != null) && (dragged.GetType() == typeof(ObstacleModel))) contextMenuStrip.Items[1].Enabled = true;
                else contextMenuStrip.Items[1].Enabled = false;                
                contextMenuLocation = location;
                return true;
            }
            return false;
        }

        public bool MouseUp(MouseEventArgs e, Matrix transform)
        {
            Point[] pp = new Point[] { e.Location };
            transform.TransformPoints(pp);
            Point location = pp[0];

            if (e.Button == MouseButtons.Left)
            {
                state = State.NONE;                
            }
            return false;
        }

        public bool MouseMove(MouseEventArgs e, Matrix transform)
        {
            Point[] pp = new Point[] { e.Location };
            transform.TransformPoints(pp);
            Point location = pp[0];
            
            if (state == State.DRAGGEDINSIDE)
            {
                lock (dragged)
                {
                    if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        if (((DateTime.Now - lastCtrlTime).Milliseconds > 200) || ((lastCtrlLocation.X - location.X) * (lastCtrlLocation.X - location.X) + (lastCtrlLocation.Y - location.Y) * (lastCtrlLocation.Y - location.Y) > 100))
                        {
                            dragged.SetPosition(location, true);
                            lastCtrlTime = DateTime.Now;
                            lastCtrlLocation = location;
                        }
                    }
                    else
                    {
                        dragged.SetPosition(location, false);
                    }
                }
                TakeSample();                
                return true;
            }
            else if (state == State.DRAGGEDOUTSIDE)
            {
                dragged.SetSecondParameterAgainstPosition(location);
                TakeSample();                
                return true;
            }
            else if ((state == State.NONE) && (e.Button == MouseButtons.None))
            {
                IDragable inside = getInsideDragabe(location);
                IDragable outside = getOutsideDragabe(location);
                if (inside != null)
                {
                    if (inselected != inside)
                    {
                        graphicControl.Cursor = Cursors.Hand;
                        if (inselected != null) inselected.SetSelectedState(0, 0);
                        inselected = inside;
                        outselected = null;
                        inselected.SetSelectedState(1, 0);                                                
                    }
                    return true;
                }
                else if (outside != null)
                {
                    if (outselected != outside)
                    {
                        graphicControl.Cursor = Cursors.NoMove2D;
                        if (outselected != null) outselected.SetSelectedState(0, 0);
                        outselected = outside;
                        inselected = null;
                        outselected.SetSelectedState(0, 1);                                          
                    }
                    return true;
                }
                else
                {
                    if ((outselected != null) || (inselected != null))
                    {
                        graphicControl.Cursor = Cursors.Arrow;
                        if (outselected != null) outselected.SetSelectedState(0, 0);
                        if (inselected != null) inselected.SetSelectedState(0, 0);
                        outselected = null;
                        inselected = null;                        
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public void newObstacle(object sender, EventArgs e)
        {
            ObstacleModel om = new ObstacleModel(contextMenuLocation, 30);
            lock (obstacles)
            {
                obstacles.Add(om);
            }
            dragables.Add(om);            
        }

        public void deleteObstacle(object sender, EventArgs e)
        {
            lock (obstacles)
            {
                obstacles.Remove((ObstacleModel)dragged);
            }
            dragables.Remove(dragged);                        
        }

        public CarModelState  GetCarState()
        {
            return sampledCarState;
        }

        public FinishState GetFinishState(int iteration)
        {
            return finish.state;
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
       
        public CarModel GetCarModel()
        {
            return model;
        }

        public FinishModel GetFinishModel(int iteration)
        {
            if (iteration > 0) return new FinishModel(GetFinishState(iteration));
            else return finish;
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
                        list.Add(new ObstacleModel(os.pp.position, os.radius));
                    }
                }
                return list;
            }
            else
            {
                return obstacles;
            }
        }

        public void TakeSample()
        {
            sampledCarState = model.state;
        }
    }
}
