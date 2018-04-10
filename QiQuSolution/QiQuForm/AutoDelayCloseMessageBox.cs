using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QiQuForm
{
    public class AutoDelayCloseMessageBox
    {
        private static Timer t;
        private static Form owner;

        //private Timer t;
        //private Form owner;

        static AutoDelayCloseMessageBox()
        {
            owner = new Form();
            t = new Timer();
            t.Tick += T_Tick;
        }

        private static void T_Tick(object sender, EventArgs e)
        {
            if (owner != null)
            {
                if (!owner.IsDisposed)
                {
                    owner.Dispose();
                }
                owner.Close();
            }
            t.Stop();
        }

        private AutoDelayCloseMessageBox()
        {
            owner = new Form();
            t = new Timer();
            t.Tick += T_Tick;
        }


        /// <summary>
        /// 在指定对象的前面显示具有指定文本和标题的消息框，并在指定的时间之后自动关闭。
        /// </summary>
        /// <param name="text">要在消息框中显示的文本。</param>
        /// <param name="caption">要在消息框的标题栏中显示的文本。</param>
        /// <param name="delayCloseMilliseconds">自动关闭的延迟时间。</param>
        public static void Show(string text, string caption, int delayCloseMilliseconds)
        {
            if (delayCloseMilliseconds > 0)
            {
                if (owner == null || owner.IsDisposed)
                {
                    owner = new Form();
                }
                t.Interval = delayCloseMilliseconds;
                t.Start();
                MessageBox.Show(owner, text, caption);
            }
            else
            {
                MessageBox.Show(text, caption);
            }
        }


        /// <summary>
        /// 在指定对象的前面显示具有指定文本的消息框，并在指定的时间之后自动关闭。
        /// </summary>
        /// <param name="text">要在消息框中显示的文本。</param>
        /// <param name="delayCloseMilliseconds">自动关闭的延迟时间。</param>
        public static void Show(string text, int delayCloseMilliseconds)
        {
            if (delayCloseMilliseconds > 0)
            {
                if (owner == null || owner.IsDisposed)
                {
                    owner = new Form();
                }
                t.Interval = delayCloseMilliseconds;
                t.Start();
                MessageBox.Show(owner, text);
            }
            else
            {
                MessageBox.Show(text);
            }
        }
    }
}
