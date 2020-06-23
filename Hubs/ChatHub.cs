using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using R.A.D.Models;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Identity;
using R.A.D.Models.Chat;

namespace R.A.D.Hubs
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        ApplicationDbContext _Context = new ApplicationDbContext();

        static List<MessageDetail> messages = new List<MessageDetail>();
        static List<MessageDetail> returnlstMessages = new List<MessageDetail>();
        //connected
        public override Task OnConnected()
        {
            try
            {
                var userId = HttpContext.Current.User.Identity.GetUserId();
                _Context.UsersConnection.Add(new UsersConnection
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId
                });
                Console.WriteLine(userId);
                Console.WriteLine(Context.ConnectionId);
                _Context.SaveChanges();
            }
            catch { }
            return base.OnConnected();
        }
        //disconnected
        public override Task OnDisconnected(bool stopCalled)
        {
            //disconected
            var current = _Context.UsersConnection.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (current != null)
            {
                _Context.UsersConnection.Remove(current);
                _Context.SaveChanges();
            }
            return base.OnDisconnected(stopCalled);
        }

        //sendMessages
        public void sendPrivateMessage(string toUserId, string message)
        {
            try
            {
                string userid;
                string fromconnectionid = Context.ConnectionId;
                string fromUserId = (_Context.UsersConnection.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserId).FirstOrDefault());
                var Adminconnections = _Context.UsersConnection.Where(u => u.UserId == toUserId).Select(u => u.ConnectionId).ToList();
                string toconnectionid = Adminconnections.LastOrDefault();

                var FromUserName = _Context.Users.Where(u => u.Id == fromUserId).Select(u => u.Name).FirstOrDefault();



                Clients.Client(toconnectionid).newMessage(FromUserName, fromUserId, message);
                Clients.Client(fromconnectionid).newselfMessage(FromUserName, fromUserId, message);
                if (toUserId.CompareTo("8b61c0f9-7c10-4100-8eef-783e65dbf13b") == 0)
                {
                    userid = fromUserId;
                }
                else
                {
                    userid = toUserId;
                }
                MessageDetail messagetxt = new MessageDetail
                {
                    UserId = userid,
                    messagetext = message,
                    userName = FromUserName,
                };
                messages.Add(messagetxt);
            }
            catch { }

        }
        public void getMessages(string touserid)
        {
            List<MessageDetail> CurrentChatMessages = new List<MessageDetail>();

            for (int i = 0; i < messages.Count; i++)
            {
                if (touserid.CompareTo(messages[i].UserId) == 0)
                {
                    MessageDetail Fmessage = new MessageDetail();
                    Fmessage.UserId = messages[i].UserId;
                    Fmessage.messagetext = messages[i].messagetext;
                    Fmessage.userName = messages[i].userName;
                    CurrentChatMessages.Add(Fmessage);
                }
            }

            Clients.Caller.getAllMessages(CurrentChatMessages);

        }

    }

  
}
