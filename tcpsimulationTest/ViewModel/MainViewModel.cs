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
            tcpvariable.HeartbeatForm = $"{ tcpvariable.HeartbeatForm.PadRight(106,'0')}b";
            cpt = 0;



            tcpvariable.Source = new ObservableCollection<string>() { "ready", "noready" };
            tcpvariable.CboxSelected = new string[10];
            tcpvariable.boxN = new string[9] { "L1","L2","L3","L4","L5","L6","L7","L8","L9"};
            tcpvariable.Textboxsheif = new string[9] { "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif"};

            tcpvariable.Datagrid_List = new ObservableCollection<Paneldata>();

        }

        
        //计数器
        private int cpt;

        private volatile bool _isBusing = false;

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

       // public ObservableCollection<Paneldata> tcpvariable.Datagrid_List { get; set; } = new ObservableCollection<Paneldata>();


        public ICommand ConnectCommand => new RelayCommand<Button>(OnConnect);

        /// <summary>
        /// 连接按钮
        /// </summary>
        /// <param name="button"></param>
        private void OnConnect(Button button)
        {
           if(button.Content.Equals("连接"))
            {
                button.Content = "关闭连接";
                if (0== _tcpIpClient.Connect(tcpvariable.IPTarget, tcpvariable.Port))
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
        /// 接收数据后的处理事件
        /// </summary>
        /// <param name="recBytes">在tcpConnect类里面已经暴露出来了</param>
        private void _tcpIpClient_RecipeFonction(List<byte> recBytes)
        {
            if (_isBusing)
            {
                //如果处理中，反馈datang
                //_tcpIpClient.SendStringData("datang");
            }
            else
            {
                //  _isBusing = true;
               

               

                string recStr = System.Text.Encoding.Default.GetString(recBytes.ToArray()).TrimEnd('\0');

                //过滤开关
                if (false == tcpvariable.isChec || recStr.Contains('.'))
                {
                    //收到信息以后直接先显示
                    tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{recStr}\r\n";
                }


               



                #region 回答心跳
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

                else //收到信息并显示 非心跳 或比对不成功的情况
                {

                  if (recStr.Contains("."))
                    {
                        Thread.Sleep(1000);//模拟机器人处理x秒种



                        paneldata = new Paneldata() { };
                        paneldata.Lenght = recStr.Substring(3, 6); //到第8位
                        paneldata.Width = recStr.Substring(9, 6);//到第14位
                        paneldata.Epaisseur = recStr.Substring(15, 4);//到第18
                        paneldata.Speed = recStr.Substring(19, 2);//20
                        paneldata.Toolchose = recStr.Substring(21, 1);
                        paneldata.PickLevel_1 = recStr.Substring(22, 1);
                        paneldata.PickLevel_2 = recStr.Substring(23, 2);   //23-24

                        paneldata.Pickxyz =$"{recStr.Substring(25, 7)}*{recStr.Substring(32, 7)}*{recStr.Substring(39, 7)}";
                        paneldata.PickxyzRatation =$"{recStr.Substring(46, 6)}*{recStr.Substring(52, 6)}*{recStr.Substring(58, 6)}";

                        paneldata.PutLevel_1 = recStr.Substring(64, 1);
                        paneldata.PutLevel_2 = recStr.Substring(65, 2);//66
                        paneldata.Putxyz = $"{recStr.Substring(67, 7)}*{recStr.Substring(74, 7)}*{recStr.Substring(81, 7)}";
                        paneldata.PutxyzRotation = $"{recStr.Substring(88, 6)}*{recStr.Substring(94, 6)}*{recStr.Substring(100, 6)}";




                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            //要处理UI的逻辑部分
                            tcpvariable.Datagrid_List.Add(paneldata);
                        }));



                       


                        var sendresult = _tcpIpClient.SendStringData(tcpvariable.Textboxdataok);

                        if(0== sendresult)//dataok 回复
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.Textboxdataok}\r\n";
                        }
                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok回复失败\r\n";
                        }
                        cpt++;
                    }
                  else
                    {
                        tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n信息比对不成功，请检查心跳比对信息\r\n";
                        cpt++;
                    }
                   
                }
                

              
                if (cpt == 30)
                {
                    tcpvariable.Texbox2 = "";
                }



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
        void Analysecode(string recStr)
        {

        }





    }
}