using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using tcpsimulationTest.Model;

namespace tcpsimulationTest.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        public static MainViewModel MainWindowStaticInst;

        public MainViewModel()
        {

            //由tcpConnect类实例化后的回调
            _tcpIpClient.RecDataHandleCallBack += _tcpIpClient_RecipeFonction;
            //心跳接收
            tcpvariable.HeartbeatForm = $"{ tcpvariable.HeartbeatForm.PadRight(106, '0')}b";
            cpt = 0;

            tcpvariable.Source = new ObservableCollection<string>() { "ready", "noready" };
            tcpvariable.CboxSelected = new string[10];
            tcpvariable.boxN = new string[9] { "L1", "L2", "L3", "L4", "L5", "L6", "L7", "L8", "L9" };
            tcpvariable.Textboxsheif = new string[9] { "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif" };
            tcpvariable.Datagrid_List = new ObservableCollection<Paneldata>();


            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = false; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
           

        }


        //计数器
        private int cpt;

        //机械指示是否忙碌
        private volatile bool _isBusing = true;

       // private int countoftime=0;

        /// <summary>
        /// tcp类的实例化
        /// </summary>
        readonly tcpConnect _tcpIpClient = new tcpConnect();

        /// <summary>
        /// tcpVariable类实例化
        /// </summary>
        public tcpVariable tcpvariable { get; set; } = new tcpVariable();

        /// <summary>
        /// 板件信息类实例化
        /// </summary>
        public Paneldata paneldata { get; set; } = new Paneldata();

        //计时器
        public  System.Timers.Timer t = new System.Timers.Timer(10000);

        static object locker = new object();
        string repiter = "";


        public ICommand ConnectCommand => new RelayCommand<Button>(OnConnect);

        /// <summary>
        /// 连接按钮
        /// </summary>
        /// <param name="button"></param>
        private void OnConnect(Button button)
        {
            if (button.Content.Equals("连接"))
            {
                button.Content = "关闭连接";
                if (0 == _tcpIpClient.Connect(tcpvariable.IPTarget, tcpvariable.Port))
                    tcpvariable.State = "已连接";
                else
                {
                    tcpvariable.Texbox1 = "连接超时，返回-1";
                    tcpvariable.State = "连接失败";
                    button.Content = "连接";//按钮恢复原状态
                }
            }
            else
            {
                _tcpIpClient.DisConnect();
                button.Content = "连接";
                tcpvariable.State = "未连接";
            }
        }

        public ICommand checkboxCommand => new RelayCommand<string>(OnChecked);
        /// <summary>
        /// 是否过滤心跳
        /// </summary>
        /// <param name="para"></param>

        private void OnChecked(string para)
        {//清空一次textbox
            tcpvariable.Texbox2 = "";
        }


        /// <summary>
        /// 计时器到时间执行的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        {

            _isBusing = false;

        }



        /// <summary>
        /// 接收数据后的处理事件
        /// </summary>
        /// <param name="recBytes">在tcpConnect类里面已经暴露出来了</param>
        private void _tcpIpClient_RecipeFonction(List<byte> recBytes)
        {


            string recStr = System.Text.Encoding.Default.GetString(recBytes.ToArray()).TrimEnd('\0');

            //过滤开关
            if (false == tcpvariable.isChec || recStr.Contains('.'))
            {
                //收到信息以后直接先显示
                tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{recStr}\r\n";
            }


            #region 回复心跳的情况
            if (recStr == tcpvariable.HeartbeatForm) //心跳比对成功
            {

                Thread.Sleep(1000);//模拟机器人处理x秒种

                var t = CollectionMachine();
                var sendresult = _tcpIpClient.SendStringData(t);

                if (0 == sendresult)
                {
                    t = $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{CollectionMachine()}\r\n";
                }
                else
                {
                    t = $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n心跳应答回复发送失败\r\n";
                }
                //过滤开关
                if (false == tcpvariable.isChec)
                {
                    cpt++;
                    tcpvariable.Texbox2 += t;
                }
            }
            #endregion

            #region 回复其他情况
            else //收到信息并显示 非心跳 或比对不成功的情况
            {

                if (recStr.Contains("."))
                {
                    lock (locker)
                    {
                        t.Enabled = true; //是否触发Elapsed事件
                        t.Start();

                     }



                if (!_isBusing)
                    {

                        #region 完成信息的回复
                        Thread.Sleep(200);

                        var sendresultsuss = _tcpIpClient.SendStringData(tcpvariable.TextboxdataSendbackok);

                        if (0 == sendresultsuss)//模拟器回复对方“ok”回复成功
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.TextboxdataSendbackok}\r\n";


                            if (tcpvariable.Datagrid_List.Count() != 0)
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    //要处理UI的逻辑部分
                                    tcpvariable.Datagrid_List[tcpvariable.Datagrid_List.Count() - 1].Stuts = "处理完成";
                                }));
                            }


                        }

                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok回复失败\r\n";
                        }

                        cpt++;

                        #endregion
                        //发送接收任务信息后 打开忙碌状态
                        _isBusing = true;

                    }
                    else
                    {
                        #region 繁忙信息回复

                        Thread.Sleep(200);
                        var sendresult = _tcpIpClient.SendStringData(tcpvariable.TextboxdataSendbackBusy);

                        if (0 == sendresult)//模拟器回复对方“繁忙”回复成功
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.TextboxdataSendbackBusy}\r\n";

                            var paneldata = Analysecode(recStr);
                           

                            if (repiter!= recStr)
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    //要处理UI的逻辑部分
                                    tcpvariable.Datagrid_List.Add(paneldata);
                                }));
                            }

                            //上一次接收的信息是否相同
                              repiter = recStr;
                        }
                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok回复失败\r\n";
                        }

                        cpt++;
                        #endregion
                    }




                }
                else
                {
                    tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n收到控制软件的信息，但信息与信息框比对不成功，请检查心跳比对信息\r\n";
                    cpt++;
                }

            }

            #endregion

            //清空显示框
            if (cpt == 30)
            {
                tcpvariable.Texbox2 = "";
            }

                return;
        }



        /// <summary>
        /// 回复客户端信息
        /// </summary>
        /// <returns></returns>
        public string CollectionMachine()
        {
            string total_text = "";
            total_text = $"{tcpvariable.Textboxarm}:{tcpvariable.TextboxarmL}:{tcpvariable.CboxSelected[0]}," +
                       $"{tcpvariable.Textboxsheif[0]}:{tcpvariable.boxN[0]}:{tcpvariable.CboxSelected[1]}," +
                       $"{tcpvariable.Textboxsheif[1]}:{tcpvariable.boxN[1]}:{tcpvariable.CboxSelected[2]}," +
                       $"{tcpvariable.Textboxsheif[2]}:{tcpvariable.boxN[2]}:{tcpvariable.CboxSelected[3]}," +
                       $"{tcpvariable.Textboxsheif[3]}:{tcpvariable.boxN[3]}:{tcpvariable.CboxSelected[4]}," +
                       $"{tcpvariable.Textboxsheif[4]}:{tcpvariable.boxN[4]}:{tcpvariable.CboxSelected[5]}," +
                       $"{tcpvariable.Textboxsheif[5]}:{tcpvariable.boxN[5]}:{tcpvariable.CboxSelected[6]}," +
                       $"{tcpvariable.Textboxsheif[6]}:{tcpvariable.boxN[6]}:{tcpvariable.CboxSelected[7]}," +
                       $"{tcpvariable.Textboxsheif[7]}:{tcpvariable.boxN[7]}:{tcpvariable.CboxSelected[8]}," +
                       $"{tcpvariable.Textboxsheif[8]}:{tcpvariable.boxN[8]}:{tcpvariable.CboxSelected[9]}";
               

            return total_text;
        }

        /// <summary>
        /// 对收到的信息进行解析
        /// </summary>
        Paneldata Analysecode(string recstr)
        {
            paneldata = new Paneldata() { };

            paneldata.Stuts =  "机器人处理中";
            paneldata.Lenght = recstr.Substring(3, 6); //到第8位
            paneldata.Width = recstr.Substring(9, 6);//到第14位
            paneldata.Epaisseur = recstr.Substring(15, 4);//到第18
            paneldata.Speed = recstr.Substring(19, 2);//20
            paneldata.Toolchose = recstr.Substring(21, 1);
            paneldata.PickLevel_1 = recstr.Substring(22, 1);
            paneldata.PickLevel_2 = recstr.Substring(23, 2);   //23-24

            paneldata.Pickxyz = $"{recstr.Substring(25, 7)}*{recstr.Substring(32, 7)}*{recstr.Substring(39, 7)}";
            paneldata.PickxyzRatation = $"{recstr.Substring(46, 6)}*{recstr.Substring(52, 6)}*{recstr.Substring(58, 6)}";

            paneldata.PutLevel_1 = recstr.Substring(64, 1);
            paneldata.PutLevel_2 = recstr.Substring(65, 2);//66
            paneldata.Putxyz = $"{recstr.Substring(67, 7)}*{recstr.Substring(74, 7)}*{recstr.Substring(81, 7)}";
            paneldata.PutxyzRotation = $"{recstr.Substring(88, 6)}*{recstr.Substring(94, 6)}*{recstr.Substring(100, 6)}";

            return paneldata;
        }





    }
}