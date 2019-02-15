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

            //��tcpConnect��ʵ������Ļص�
            _tcpIpClient.RecDataHandleCallBack += _tcpIpClient_RecipeFonction;
            //��������
            tcpvariable.HeartbeatForm = $"{ tcpvariable.HeartbeatForm.PadRight(106, '0')}b";
            cpt = 0;

            tcpvariable.Source = new ObservableCollection<string>() { "ready", "noready" };
            tcpvariable.CboxSelected = new string[10];
            tcpvariable.boxN = new string[9] { "L1", "L2", "L3", "L4", "L5", "L6", "L7", "L8", "L9" };
            tcpvariable.Textboxsheif = new string[9] { "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif" };
            tcpvariable.Datagrid_List = new ObservableCollection<Paneldata>();


            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = false; //ÿ��ָ��ʱ��Elapsed�¼��Ǵ���һ�Σ�false��������һֱ������true��
           

        }


        //������
        private int cpt;

        //��еָʾ�Ƿ�æµ
        private volatile bool _isBusing = true;

       // private int countoftime=0;

        /// <summary>
        /// tcp���ʵ����
        /// </summary>
        readonly tcpConnect _tcpIpClient = new tcpConnect();

        /// <summary>
        /// tcpVariable��ʵ����
        /// </summary>
        public tcpVariable tcpvariable { get; set; } = new tcpVariable();

        /// <summary>
        /// �����Ϣ��ʵ����
        /// </summary>
        public Paneldata paneldata { get; set; } = new Paneldata();

        //��ʱ��
        public  System.Timers.Timer t = new System.Timers.Timer(10000);

        static object locker = new object();
        string repiter = "";


        public ICommand ConnectCommand => new RelayCommand<Button>(OnConnect);

        /// <summary>
        /// ���Ӱ�ť
        /// </summary>
        /// <param name="button"></param>
        private void OnConnect(Button button)
        {
            if (button.Content.Equals("����"))
            {
                button.Content = "�ر�����";
                if (0 == _tcpIpClient.Connect(tcpvariable.IPTarget, tcpvariable.Port))
                    tcpvariable.State = "������";
                else
                {
                    tcpvariable.Texbox1 = "���ӳ�ʱ������-1";
                    tcpvariable.State = "����ʧ��";
                    button.Content = "����";//��ť�ָ�ԭ״̬
                }
            }
            else
            {
                _tcpIpClient.DisConnect();
                button.Content = "����";
                tcpvariable.State = "δ����";
            }
        }

        public ICommand checkboxCommand => new RelayCommand<string>(OnChecked);
        /// <summary>
        /// �Ƿ��������
        /// </summary>
        /// <param name="para"></param>

        private void OnChecked(string para)
        {//���һ��textbox
            tcpvariable.Texbox2 = "";
        }


        /// <summary>
        /// ��ʱ����ʱ��ִ�еĺ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_TimesUp(object sender, System.Timers.ElapsedEventArgs e)
        {

            _isBusing = false;

        }



        /// <summary>
        /// �������ݺ�Ĵ����¼�
        /// </summary>
        /// <param name="recBytes">��tcpConnect�������Ѿ���¶������</param>
        private void _tcpIpClient_RecipeFonction(List<byte> recBytes)
        {


            string recStr = System.Text.Encoding.Default.GetString(recBytes.ToArray()).TrimEnd('\0');

            //���˿���
            if (false == tcpvariable.isChec || recStr.Contains('.'))
            {
                //�յ���Ϣ�Ժ�ֱ������ʾ
                tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{recStr}\r\n";
            }


            #region �ظ����������
            if (recStr == tcpvariable.HeartbeatForm) //�����ȶԳɹ�
            {

                Thread.Sleep(1000);//ģ������˴���x����

                var t = CollectionMachine();
                var sendresult = _tcpIpClient.SendStringData(t);

                if (0 == sendresult)
                {
                    t = $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{CollectionMachine()}\r\n";
                }
                else
                {
                    t = $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n����Ӧ��ظ�����ʧ��\r\n";
                }
                //���˿���
                if (false == tcpvariable.isChec)
                {
                    cpt++;
                    tcpvariable.Texbox2 += t;
                }
            }
            #endregion

            #region �ظ��������
            else //�յ���Ϣ����ʾ ������ ��ȶԲ��ɹ������
            {

                if (recStr.Contains("."))
                {
                    lock (locker)
                    {
                        t.Enabled = true; //�Ƿ񴥷�Elapsed�¼�
                        t.Start();

                     }



                if (!_isBusing)
                    {

                        #region �����Ϣ�Ļظ�
                        Thread.Sleep(200);

                        var sendresultsuss = _tcpIpClient.SendStringData(tcpvariable.TextboxdataSendbackok);

                        if (0 == sendresultsuss)//ģ�����ظ��Է���ok���ظ��ɹ�
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.TextboxdataSendbackok}\r\n";


                            if (tcpvariable.Datagrid_List.Count() != 0)
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    //Ҫ����UI���߼�����
                                    tcpvariable.Datagrid_List[tcpvariable.Datagrid_List.Count() - 1].Stuts = "�������";
                                }));
                            }


                        }

                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok�ظ�ʧ��\r\n";
                        }

                        cpt++;

                        #endregion
                        //���ͽ���������Ϣ�� ��æµ״̬
                        _isBusing = true;

                    }
                    else
                    {
                        #region ��æ��Ϣ�ظ�

                        Thread.Sleep(200);
                        var sendresult = _tcpIpClient.SendStringData(tcpvariable.TextboxdataSendbackBusy);

                        if (0 == sendresult)//ģ�����ظ��Է�����æ���ظ��ɹ�
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.TextboxdataSendbackBusy}\r\n";

                            var paneldata = Analysecode(recStr);
                           

                            if (repiter!= recStr)
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    //Ҫ����UI���߼�����
                                    tcpvariable.Datagrid_List.Add(paneldata);
                                }));
                            }

                            //��һ�ν��յ���Ϣ�Ƿ���ͬ
                              repiter = recStr;
                        }
                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok�ظ�ʧ��\r\n";
                        }

                        cpt++;
                        #endregion
                    }




                }
                else
                {
                    tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n�յ������������Ϣ������Ϣ����Ϣ��ȶԲ��ɹ������������ȶ���Ϣ\r\n";
                    cpt++;
                }

            }

            #endregion

            //�����ʾ��
            if (cpt == 30)
            {
                tcpvariable.Texbox2 = "";
            }

                return;
        }



        /// <summary>
        /// �ظ��ͻ�����Ϣ
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
        /// ���յ�����Ϣ���н���
        /// </summary>
        Paneldata Analysecode(string recstr)
        {
            paneldata = new Paneldata() { };

            paneldata.Stuts =  "�����˴�����";
            paneldata.Lenght = recstr.Substring(3, 6); //����8λ
            paneldata.Width = recstr.Substring(9, 6);//����14λ
            paneldata.Epaisseur = recstr.Substring(15, 4);//����18
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