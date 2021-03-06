﻿using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Pushs.GCM
{
    public class GCMPush:Push
    {
        private GcmConfiguration Configuration;
        private GcmServiceBroker Broker;
        private List<String> Tokens;


        public GCMPush()
        {
            this.Configuration = new GcmConfiguration(GCMResources.Sender_id, GCMResources.Api_key, null);
            this.Broker = new GcmServiceBroker(this.Configuration);
            this.Broker.OnNotificationFailed+=(Notification,aggregateEx)=>{

                aggregateEx.Handle(GCMException =>
                {
                    if(GCMException is GcmNotificationException){
                         GcmNotificationException notificationException = (GcmNotificationException)GCMException;

                         var gcmNotification = notificationException.Notification;
                         var description = notificationException.Description;
                         Console.WriteLine ("Error 1");

                    }
                    else if (GCMException is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)GCMException;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine(String.Format("Success:{0}", succeededNotification.MessageId));
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine(String.Format("GCM Notification Failed: ID={0}", n.MessageId));
                        }
                    }
                    else  if (GCMException is DeviceSubscriptionExpiredException) {
                        var expiredException = (DeviceSubscriptionExpiredException)GCMException;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine (string.Format("Device RegistrationId Expired: {0}",oldId));

                        if (!(string.IsNullOrWhiteSpace(newId))) {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine (String.Format("Device RegistrationId Changed To: {0}",newId));
                            }
                        } else if (GCMException is RetryAfterException) {
                            var retryException = (RetryAfterException)GCMException;
                            // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                            Console.WriteLine 
                                (string.Format("GCM Rate Limited, don't send more until after {0})",retryException.RetryAfterUtc));
                        } else {
                            Console.WriteLine ("GCM Notification Failed for some unknown reason");
                        }
                    
                    return true;
                });



                this.Broker.OnNotificationSucceeded += (notification) =>
                {
                    Console.WriteLine("GCM Notification Sent!");
                };
            
            
            
            
            };
            this.Tokens= new List<string>();
            this.Broker.Start();
        }

        public override void Send(String Title, String Message, String Data)
        {
            String _message = "{\"title\" : \"{0}\" ,\"message\" : \"{1}\", \"data\" : \"{2}\"}";
            _message = _message.Replace("{0}", Title);
            _message = _message.Replace("{1}", Message);
            _message = _message.Replace("{2}", Data);

            this.Broker.QueueNotification(new GcmNotification
            {

               RegistrationIds = this.Tokens,
               Data = JObject.Parse(_message)

            });
        }

        // "fPumEGyIg_0:APA91bHp9KxqB_dYS4GaG-xW9wOmmpBXxuwFsA_FM1MT2w2vLEekPMU-KlvR0UYs9xm8-Z0SZIOvLbnNzMaHxHi7ZNpVWi9xQINqLd5nOP-0HSEcT9kZuFpn9XID28mAELR3lEK7ck4H"
        //  "APA91bEryvniHWx3vgBQSBktNNjA971X3IowTLdV0AeDhn0dNIToI7X3IZmvK8xmSjMeOofEcl4GOmU0Pis2UME31PD_mn9yQc8ybkCuAWCWLjf-dh5szZTkuO7T-TjxKCtyoctUjJW3"

        public override void AddToken(string Token)
        {
            this.Tokens.Add(Token);
        }
    }
}