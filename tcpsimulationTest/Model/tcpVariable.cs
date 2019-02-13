using GalaSoft.MvvmLight.Command;
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
   public class tcpVariable : INotifyPropertyChanged
    {

        public tcpVariable()
        {
           
            _heartbeat = "a";

        } 

        

        private string _ipmachine="";
        public string IPMachine
        {
            get { return _ipmachine; }
            set
            {
                if (_ipmachine == value)
                    return;
                _ipmachine = value;
                OnPropertyChanged("IPMachine");
            }
        }





        private string _itarget = "192.168.0.33";
        public string IPTarget
        {
            get { return _itarget; }
            set
            {
                if (_itarget == value)
                    return;
                _itarget = value;
                OnPropertyChanged("IPTarget");
            }
        }




        private string _port = "9528";
        public string Port
        {
            get { return _port; }
            set
            {
                if (_port == value)
                    return;
                _port = value;
                OnPropertyChanged("Port");
            }
        }


        private string _state = "未连接";
        public string State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;
                _state = value;
                OnPropertyChanged("State");
            }
        }



        private string _texbox1;
        /// <summary>
        /// 通信状态
        /// </summary>
        public string Texbox1
        {
            get { return _texbox1; }
            set
            {
                if (_texbox1 == value)
                    return;
                _texbox1 = value;
                OnPropertyChanged("Texbox1");
            }
        }



        private string _texbox2;
        /// <summary>
        /// 通信内容
        /// </summary>
        public string Texbox2
        {
            get { return _texbox2; }
            set
            {
                if (_texbox2 == value)
                    return;
                _texbox2 = value;
                OnPropertyChanged("Texbox2");
            }
        }


        private string _heartbeat;
        /// <summary>
        /// 心跳输入框
        /// </summary>
        public string HeartbeatForm
        {
            get { return _heartbeat; }
            set
            {
                if (_heartbeat == value)
                    return;
                _heartbeat = value;
                OnPropertyChanged("HeartbeatForm");
            }
        }



        private string _tbarm = "arm";
        /// <summary>
        /// textbox arm
        /// </summary>
        public string Textboxarm
        {
            get { return _tbarm; }
            set
            {
                if (_tbarm == value)
                    return;
                _tbarm = value;
                OnPropertyChanged("Textboxarm");
            }
        }


        private string _tbarmL = "1";
        /// <summary>
        /// textbox arm 1
        /// </summary>
        public string TextboxarmL
        {
            get { return _tbarmL; }
            set
            {
                if (_tbarmL == value)
                    return;
                _tbarmL = value;
                OnPropertyChanged("TextboxarmL");
            }
        }


        private string[] _tbsheif;
        /// <summary>
        /// textbox sheif
        /// </summary>
        public string[] Textboxsheif
        {
            get { return _tbsheif; }
            set
            {
                if (_tbsheif == value)
                    return;
                _tbsheif = value;
                OnPropertyChanged("Textboxsheif");
            }
        }




        private string[] _n;
        /// <summary>
        /// textbox sheif L
        /// </summary>
        public string[] boxN
        {
            get { return _n; }
            set
            {
                if (_n == value)
                    return;
                _n = value;
                OnPropertyChanged("boxN");
            }
        }

        private bool checkvariable=false;
        /// <summary>
        /// checkbox
        /// </summary>
        public bool isChec
        {
            get { return checkvariable; }
            set
            {
                if (checkvariable == value)
                    return;
                checkvariable = value;
                OnPropertyChanged("isChec");
            }
        }

        private string _tbok = "dataok";
        /// <summary>
        /// 回复dataok
        /// </summary>
        public string Textboxdataok
        {
            get { return _tbok; }
            set
            {
                if (_tbok == value)
                    return;
                _tbok = value;
                OnPropertyChanged("Textboxdataok");
            }
        }



        private ObservableCollection<string> _source;
        /// <summary>
        /// combox 
        /// </summary>
        public ObservableCollection<string> Source
        {
            get { return _source; }
            set
            {
                if (_source == value)
                    return;
                _source = value;
                OnPropertyChanged("Source");
            }
        }

        private ObservableCollection<Paneldata> _grid;
        /// <summary>
        /// datagrid
        /// </summary>
        public ObservableCollection<Paneldata> Datagrid_List
        {
            get { return _grid; }
            set
            {
                if (_grid == value)
                    return;
                _grid = value;
                OnPropertyChanged("Datagrid_List");
            }
        }





        private string[] _cboxselected;
        /// <summary>
        /// combobox arm
        /// </summary>
        public string[] CboxSelected
        {
            get { return _cboxselected; }
            set
            {
                if (_cboxselected == value)
                    return;
                _cboxselected = value;
                OnPropertyChanged("CboxSelected");
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }

    public class Paneldata : INotifyPropertyChanged
    {

        private string _lenght;
        public string Lenght
        {
            get { return _lenght; }
            set
            {
                if (_lenght == value)
                    return;
                _lenght = value;
                OnPropertyChanged("Lenght");
            }
        }


        private string _width;
        public string Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;
                _width = value;
                OnPropertyChanged("Width");
            }

        }
        public string Epaisseur;
        public string Speed;
        public string Toolchose;
        public string PickLevel_1;
        public string PickLevel_2;
        public string Pickxyz;
        public string PickxyzRatation;

        public string PutLevel_1;
        public string PutLevel_2;
        public string Putxyz;
        public string PutxyzRotation;




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }


}


