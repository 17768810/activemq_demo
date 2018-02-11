using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace ActiveMQ.Test.WindowsForms
{
    public partial class Form1 : Form
    {
        private readonly IConnectionFactory factory;
        public Form1()
        {
            InitializeComponent();

            //failover:(tcp://172.18.20.132:61616)?timeout=5000&jms.useAsyncSend=true
            //初始化工厂，这里默认的URL是不需要修改的
            factory = new ConnectionFactory("tcp://172.18.20.132:61616");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //通过工厂建立连接
            using (IConnection connection = factory.CreateConnection())
            {
                //通过连接创建Session会话
                using (ISession session = connection.CreateSession())
                {
                    //通过会话创建生产者，方法里面new出来的是MQ中的Queue
                    IMessageProducer prod = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue(textBox_queueName.Text.Trim()));
                    //创建一个发送的消息对象
                    ITextMessage message = prod.CreateTextMessage();
                    //给这个对象赋实际的消息
                    message.Text = textBox1.Text;
                    //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                    //message.Properties.SetString("filter", "demo");
                    //生产者把消息发送出去，几个枚举参数MsgDeliveryMode是否长链，MsgPriority消息优先级别，发送最小单位，当然还有其他重载
                    prod.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                    lbMessage.Text = "发送成功!!";
                    //txtMessage.Text = "";
                    //txtMessage.Focus();
                }
            }
        }




        //public void InitConsumer()
        //{
        //    //创建连接工厂
        //    IConnectionFactory factory = new ConnectionFactory("tcp://localhost:61616");
        //    //通过工厂构建连接
        //    IConnection connection = factory.CreateConnection();
        //    //这个是连接的客户端名称标识
        //    connection.ClientId = "firstQueueListener";
        //    //启动连接，监听的话要主动启动连接
        //    connection.Start();
        //    //通过连接创建一个会话
        //    ISession session = connection.CreateSession();
        //    //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置
        //    IMessageConsumer consumer = session.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue("firstQueue"), "filter='demo'");
        //    //注册监听事件
        //    consumer.Listener += new MessageListener(consumer_Listener);
        //    //connection.Stop();
        //    //connection.Close();  

        //}

        //void consumer_Listener(IMessage message)
        //{
        //    ITextMessage msg = (ITextMessage)message;
        //    //异步调用下，否则无法回归主线程
        //    tbReceiveMessage.Invoke(new DelegateRevMessage(RevMessage), msg);

        //}

        //public delegate void DelegateRevMessage(ITextMessage message);

        //public void RevMessage(ITextMessage message)
        //{
        //    tbReceiveMessage.Text += string.Format(@"接收到:{0}{1}", message.Text, Environment.NewLine);
        //}
    }
}
