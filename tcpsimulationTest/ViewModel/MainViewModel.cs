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
            tcpvariable.HeartbeatForm = $"{ tcpvariable.HeartbeatForm.PadRight(106,'0')}b";
            cpt = 0;



            tcpvariable.Source = new ObservableCollection<string>() { "ready", "noready" };
            tcpvariable.CboxSelected = new string[10];
            tcpvariable.boxN = new string[9] { "L1","L2","L3","L4","L5","L6","L7","L8","L9"};
            tcpvariable.Textboxsheif = new string[9] { "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif", "sheif"};

        }

        
        //������
        private int cpt;

        private volatile bool _isBusing = false;

        /// <summary>
        /// tcp���ʵ����
        /// </summary>
        readonly tcpConnect _tcpIpClient = new tcpConnect();

        /// <summary>
        /// tcpVariable��ʵ����
        /// </summary>
        public tcpVariable tcpvariable { get; set; } = new tcpVariable();

        


        public ICommand ConnectCommand => new RelayCommand<Button>(OnConnect);

        /// <summary>
        /// ���Ӱ�ť
        /// </summary>
        /// <param name="button"></param>
        private void OnConnect(Button button)
        {
           if(button.Content.Equals("����"))
            {
                button.Content = "�ر�����";
                if (0== _tcpIpClient.Connect(tcpvariable.IPTarget, tcpvariable.Port))
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
        /// �������ݺ�Ĵ����¼�
        /// </summary>
        /// <param name="recBytes">��tcpConnect�������Ѿ���¶������</param>
        private void _tcpIpClient_RecipeFonction(List<byte> recBytes)
        {
            if (_isBusing)
            {
                //��������У�����datang
                //_tcpIpClient.SendStringData("datang");
            }
            else
            {
              //  _isBusing = true;
                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                //{
                //    //Ҫ����UI���߼�����
         
                //}));

                string recStr = System.Text.Encoding.Default.GetString(recBytes.ToArray()).TrimEnd('\0');

                //���˿���
                if (false == tcpvariable.isChec || recStr.Contains('.'))
                {
                    //�յ���Ϣ�Ժ�ֱ������ʾ
                    tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{recStr}\r\n";
                }

               




                #region �ش�����
                if (recStr == tcpvariable.HeartbeatForm) //�����ȶԳɹ�
                {                  
                    
                    Thread.Sleep(1000);//ģ������˴���2����
                   
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

                else //�յ���Ϣ����ʾ ������ ��ȶԲ��ɹ������
                {

                  if (recStr.Contains("."))
                    {
                        
                        var datalist=recStr.Split('.').ToList();
                        




                         var sendresult = _tcpIpClient.SendStringData(tcpvariable.Textboxdataok);

                        if(0== sendresult)//dataok �ظ�
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n{tcpvariable.Textboxdataok}\r\n";
                        }
                        else
                        {
                            tcpvariable.Texbox2 += $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\ndataok�ظ�ʧ��\r\n";
                        }
                        cpt++;
                    }
                  else
                    {
                        tcpvariable.Texbox2 += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}\n��Ϣ�ȶԲ��ɹ������������ȶ���Ϣ\r\n";
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
        void Analysecode(string recStr)
        {

        }





    }
}