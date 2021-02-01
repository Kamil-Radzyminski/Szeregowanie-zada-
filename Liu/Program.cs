using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Liu
{

    public class Task
    {
        public int id;
        public int pi;
        public int dj;
        public int rj;
        public int di;
        public int li;
        public bool done;
        public bool rdy;

        public List<Task> previous = new List<Task>();
        public List<Task> next = new List<Task>();

        public Task(int id, int pi, int dj, int rj)
        {
            this.id = id;
            this.pi = pi;
            this.dj = dj;
            this.rj = rj;
        }
    }

    public partial class Form1 : Form
    {

        public List<Task> tasks = new List<Task>();
        public int time;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            #region input

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open("hu.xlsx", FileMode.Open, FileAccess.Read))
            {

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {

                            Task t = new Task((int)reader.GetDouble(0), (int)reader.GetDouble(1), (int)reader.GetDouble(2), (int)reader.GetDouble(3));

                            if ((int)reader.GetDouble(4) != 0)
                            {
                                t.previous.Add(tasks[(int)reader.GetDouble(4) - 1]);
                                tasks[(int)reader.GetDouble(4) - 1].next.Add(t);
                            }

                            if ((int)reader.GetDouble(5) != 0)
                            {
                                t.previous.Add(tasks[(int)reader.GetDouble(5) - 1]);
                                tasks[(int)reader.GetDouble(5) - 1].next.Add(t);
                            }

                            tasks.Add(t);
                        }
                    } while (reader.NextResult());

                }
            }


            #endregion input


            foreach (Task t in tasks)
            {
                Debug.WriteLine("id: " + t.id + "  pi: " + t.pi + "  dj: " + t.dj + "  rj: " + t.rj + "  di: " + t.di + "  li: " + t.li + "  done: " + t.done);
            }

            Debug.WriteLine("START -------------------------");
            

            foreach (Task t in tasks)
            {
                t.di = t.dj;
                foreach (Task t2 in t.next)
                {
                    t.di = (int)Math.Min(t.di, t2.dj);
                }
            }

            for (int i = 0; i < 99; i++)
            {
                if (AllDone()) break;
                OnePass();
            }

            int lmax = -9999;
            foreach (Task t in tasks)
            {
                if (lmax < t.li)
                {
                    lmax = t.li;
                }
            }

            Debug.WriteLine("");
            Debug.WriteLine("DONE----------------------" + "time: " + time + " lmax: " + lmax);


            foreach (Task t in tasks)
            {
                Debug.WriteLine("id: " + t.id + "  pi: " + t.pi + "  dj: " + t.dj + "  rj: " + t.rj + "  di: " + t.di + "  li: " + t.li + "  done: " + t.done);
            }

            /*
             *         public int pi;
        public int dj;
        public int rj;
        public int di;
        public int li;
        public bool done;
            */

        }
        bool AllDone()
        {
            foreach (Task t in tasks)
            {
                if (!t.done) return false;
            }
            return true;
        }
        void OnePass()
        {
            Task lowest = tasks[0];

            foreach (Task t in tasks)
            {
                if (t.rj <= time && !t.done)
                {
                    foreach (Task t2 in tasks)
                    {
                        bool ready = true;

                        foreach (Task t3 in t2.previous)
                        {

                            if (!t3.done)
                            {
                                ready = false;
                            }

                        }

                        if (t2.di < lowest.di && t2.rj <= time && !t.done && ready)
                        {
                            lowest = t2;
                        }
                    }

                    foreach (Task t3 in tasks)
                    {
                        if (t3.id == lowest.id)
                        {

                            if (lowest.done)
                            {
                                Debug.Write(" -- ");
                                time++;
                                return;
                            }


                            if (t3.pi <= 1)
                            {
                                t3.done = true;
                                t3.di = 99999;
                                t3.pi--;
                                t3.li = time - t3.dj + 1;
                            }
                            else
                            {
                                t3.pi--;
                            }

                            Debug.Write(" " + t3.id + " ");
                            time++;
                            return;
                        }
                    }

                }

            }
            Debug.Write(" -- ");
            time++;
        }
    }
}