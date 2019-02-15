using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace tcpsimulationTest.Model
{
    [ImplementPropertyChanged]
    public class tcpVariable
    {

        public tcpVariable()
        {

            HeartbeatForm = "a";
            TextboxdataSendbackok = "dataok";

            TextboxdataSendbackBusy = "datang";//如果不是这个指令 插件会收不到 当作无回复进行处理
            isChec = false;
            TextboxarmL = "1";

            Textboxarm = "arm";
            State = "未连接";
            Port = "9528";

            IPTarget = "192.168.0.33";
            IPMachine = "";
 
        } 

        

        public string IPMachine { get; set; }





        public string IPTarget { get; set; }


        public string Port { get; set; }


        public string State { get; set; }

        /// <summary>
        /// 通信状态
        /// </summary>
        public string Texbox1 { get; set; }



        /// <summary>
        /// 通信内容
        /// </summary>
        public string Texbox2 { get; set; }


        /// <summary>
        /// 心跳输入框
        /// </summary>
        public string HeartbeatForm { get; set; }


        /// <summary>
        /// textbox arm
        /// </summary>
        public string Textboxarm { get; set; }

        /// <summary>
        /// textbox arm 1
        /// </summary>
        public string TextboxarmL { get; set; }

        /// <summary>
        /// textbox sheif
        /// </summary>
        public string[] Textboxsheif { get; set; }

        /// <summary>
        /// textbox sheif L
        /// </summary>
        public string[] boxN { get; set; }

        /// <summary>
        /// checkbox
        /// </summary>
        public bool isChec { get; set; }
        /// <summary>
        /// 回复dataok
        /// </summary>
        public string TextboxdataSendbackok { get; set; }

        public string TextboxdataSendbackBusy { get; set; }

        /// <summary>
        /// combox 
        /// </summary>
        public ObservableCollection<string> Source { get; set; }

        /// <summary>
        /// datagrid
        /// </summary>
        public ObservableCollection<Paneldata> Datagrid_List { get; set; }


        /// <summary>
        /// combobox arm
        /// </summary>
        public string[] CboxSelected { get; set; }


    }


    [ImplementPropertyChanged]
    public class Paneldata 
    {
       public Paneldata()
        {
            //Stuts = "机器人处理中";
        }

        public string Stuts { get; set;}
        public string Lenght { get; set; }     
        public string Width { get; set; }
        public string Epaisseur { get; set; }
        public string Speed { get; set; }
        public string Toolchose { get; set; }
        public string PickLevel_1 { get; set; }
        public string PickLevel_2 { get; set; }
        public string Pickxyz { get; set; }
        public string PickxyzRatation { get; set; }

        public string PutLevel_1 { get; set; }
        public string PutLevel_2 { get; set; }
        public string Putxyz { get; set; }
        public string PutxyzRotation { get; set; }


    }


}


