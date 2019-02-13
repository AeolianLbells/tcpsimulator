using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using tcpsimulationTest.Model;

namespace tcpsimulationTest.ViewModel
{
    public delegate void FeedBackRecDataHandle(List<byte> recBytes);

    public class tcpConnect
    {
        private static TcpClient _client;
        private IPAddress _ipAddress;
        private static int _bufferSize = 1024;
        private static byte[] _recDataBytesBuff = new byte[0];  //接收数据缓存
        public static bool IsConnect = false;


        public event FeedBackRecDataHandle RecDataHandleCallBack;  //接收数据处理


        /// <summary>
        /// tcpVariable类实例化
        /// </summary>
        public tcpVariable tcpvariable { get; set; } = new tcpVariable();
        public tcpConnect()
        {
            IsConnect = false;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ipaddress">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="delayTime">异步连接超时时间 ms</param>
        /// <returns>0：成功 -1:失败</returns>
        public int Connect(string ipaddress, string port, int delayTime = 200)
        {
                _ipAddress = IPAddress.Parse(ipaddress);
                 _client = new TcpClient();
            var _port = int.Parse(port);
            _client.BeginConnect(_ipAddress, _port, MyConnectThreadAsync, _client);

            DateTime lasTime = DateTime.Now;

            ////异步连接超时判断
            do
            {
                DateTime currenTime = DateTime.Now;
                var time = currenTime.Subtract(lasTime).TotalMilliseconds;
                if (time > delayTime)
                {
                  //  tcpvariable.Texbox = "连接超时,返回-1";
                    //MessageBox.Show("连接超时,返回-1");

                    return -1;
                }
            } while (!IsConnect);

            return 0;

        }

        /// <summary>
        /// 异步连接函数
        /// </summary>
        /// <param name="ar">异步操作状态</param>
       private void MyConnectThreadAsync(IAsyncResult ar)
        {
            TcpClient asyclient_result = (TcpClient)ar.AsyncState;
            try
            {            
                if (asyclient_result.Connected)
                {
                    IsConnect = true;

                    NetworkStream networkStream;

                    //连接成功，获取stream
                    try
                    {
                        networkStream = asyclient_result.GetStream();
                    }
                    catch (Exception e)
                    {
                        IsConnect = false;
                        asyclient_result.EndConnect(ar);  //终止异步连接

                        MessageBox.Show($"获取Stream异常：{e.Message}");
                        //ExceptionInfoCallBack?.Invoke($"获取Stream异常：{e.Message}");
                        return;
                    }


                    //开启异步接收
                    if (networkStream.CanRead)
                    {
                        _recDataBytesBuff = new byte[_bufferSize];
                        networkStream.BeginRead(_recDataBytesBuff, 0, _bufferSize, AsyncReadCallBack, asyclient_result);
                    }
                    else
                    {
                        MessageBox.Show($"异步读取失败，NetworkStream不支持读取");
                       // ExceptionInfoCallBack?.Invoke($"异步读取失败，NetworkStream不支持读取");
                        return;
                    }


                }
                else
                {
                    IsConnect = false;
                    asyclient_result.EndConnect(ar);  //终止异步连接           
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"获取Stream异常：{e.Message}");
                //ExceptionInfoCallBack?.Invoke($"异步连接异常:{e.Message}");
                return;
            }

        }


        /// <summary>
        /// 异步接收函数
        /// </summary>
        /// <param name="iar">异步操作状态</param>
        private void AsyncReadCallBack(IAsyncResult iar)
        {
            TcpClient client = (TcpClient)iar.AsyncState;

            if (client == null || !IsConnect)
            {
                MessageBox.Show($"异步读取失败，client为空或连接断开");
               // ExceptionInfoCallBack?.Invoke($"异步读取失败，client为空或连接断开");
                return;
            }

            //获取ns
            NetworkStream networkStream = client.GetStream();

            int readByteNum = 0;
            //结束异步读取
            try
            {
                readByteNum = networkStream.EndRead(iar);
            }
            catch (Exception e)
            {
                MessageBox.Show($"获取Stream异常：{e.Message}");
               // ExceptionInfoCallBack?.Invoke($"异步读取失败:{e.Message}");
                return;
            }



            //获取读取的数据
            if (readByteNum > 0)
            {
                byte[] buffBytes = new byte[readByteNum];
                Array.Copy(_recDataBytesBuff, 0, buffBytes, 0, readByteNum);

                //回调处理接收的数据
                RecDataHandleCallBack?.Invoke(_recDataBytesBuff.ToList());

                //继续接收
                _recDataBytesBuff = new byte[_bufferSize];

                try
                {
                    networkStream.BeginRead(_recDataBytesBuff, 0, _bufferSize, AsyncReadCallBack, client);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"获取Stream异常：{e.Message}");
                    //ExceptionInfoCallBack?.Invoke($"异步读取失败:{e.Message}");
                    return;
                }



            }
            else
            {
                DisConnect();

                MessageBox.Show($"连接断开");
                //ExceptionInfoCallBack?.Invoke($"连接断开");

            }
        }



        /// <summary>
        /// 发送字符串函数
        /// </summary>
        /// <param name="sendData">要发送的字符串数据</param>
       // [MethodImpl(MethodImplOptions.Synchronized)]
        public int SendStringData(string sendData)
        {
            if (_client == null || !IsConnect)
            {

                MessageBox.Show($"发送失败，client为空或连接断开");
                //ExceptionInfoCallBack?.Invoke($"发送失败，client为空或连接断开");
                return -1;
            }

            NetworkStream networkStream;

            //连接成功，获取stream
            try
            {
                networkStream = _client.GetStream();
            }
            catch (Exception e)
            {
                IsConnect = false;
                MessageBox.Show(e.Message);
                //ExceptionInfoCallBack?.Invoke(e.Message);
                return -1;
            }



            //发送字符串数据转换为字节
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(sendData);

            //开始发送数据
            try
            {
                networkStream.Write(byteArray, 0, byteArray.Length);
            }
            catch (Exception e)
            {
                MessageBox.Show($"发送失败:{e.Message}");
                // ExceptionInfoCallBack?.Invoke($"发送失败:{e.Message}");
                return -1;
            }

            return 0;

        }






        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            IsConnect = false;
        
            if (null != _client)
            {
                _client.Close();
                _client = null;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            Release();
            IsConnect = false;
        }





    }
}
