using Microsoft.AspNet.SignalR;
using MIdProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MIdProject
{
    public class NotificationComponent
    {
        public void RegisterNotification(DateTime currentTime)
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            string sqlCommand = @"Select [N_Id],[Notify_Name],[Notify_No] from [dbo].[Notifications] where [AddedOn] > @AddedOn";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@AddedOn", currentTime);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                }

            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                RegisterNotification(DateTime.Now);
            }
        }
        public List<Notification> GetNotifications(DateTime afterDate)
        {
            using (ProjectEntities pe = new ProjectEntities())
            {
                return pe.Notifications.Where(a => a.AddedOn > afterDate).OrderByDescending(a => a.AddedOn).ToList();
            }
        }
    }
}