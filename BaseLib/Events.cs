using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace BaseLib
{
    public class Events
    {
        public static bool addToLoggerIsNull = true;
        public static bool QuoteaddToIsNull = true;
        public event EventHandler addToLogger;

        public event EventHandler addToLogger_sharan;
        public static bool addToLoggerIsNull_sharan = true;
        public void  LogText_sharan(EventArgs e)
        {
            if (addToLogger_sharan != null)
            {
                addToLoggerIsNull_sharan = false;
                addToLogger_sharan(this, e); //Fires the event
            }
            else
            {
                addToLoggerIsNull = true;
            }

        }


        //public event EventHandler addGroupMembers;
        public static bool BindGroupMembersIsNull = true; 
        public event EventHandler addGroupNamesForStatusUpdate;
        public void BindGroupMembersToCheckedListBox(EventArgs objEventArgs)
        {
            if (addGroupNamesForStatusUpdate != null)
            {
                BindGroupMembersIsNull = false;
                addGroupNamesForStatusUpdate(this, objEventArgs); //Fires the event
            }
            else
            {
                addToLoggerIsNull = true;
            }
        }

        public static bool BindGroupMembersScraperIsNull = true;
        public event EventHandler addGroupNamesForScraper;
        public void BindGroupMembersScraperToCheckedListBox(EventArgs objEventArgs)
        {
            if (addGroupNamesForScraper != null)
            {
                BindGroupMembersScraperIsNull = false;
                addGroupNamesForScraper(this, objEventArgs); //Fires the event
            }
            else
            {
                addToLoggerIsNull = true;
            }
        }




        
        public void LogText(EventsArgs e)
        {
            if (addToLogger != null)
            {
                addToLoggerIsNull = false;
                addToLogger(this, e); //Fires the event
            }
            else
            {
                addToLoggerIsNull = true;
            }
        }

        public event EventHandler QuoteaddToLogger;


        public void QuoteaddTo(EventsArgs e)
        {
            if (QuoteaddToLogger != null)
            {
                QuoteaddToLogger(this, e); //Fires the event
            }
        }

        public event EventHandler incrementCount;

        public void IncreaseCounter(EventsArgs e)
        {
            if (incrementCount != null)
            {
                incrementCount(this, e);
            }
        }

        public event EventHandler raiseScheduler;
        public void RaiseScheduler(EventsArgs e)
        {
            if (raiseScheduler != null)
            {
                raiseScheduler(this, e); //Fires the event
            }
        }

        /// <summary>
        /// Fires the event taking "EventsArgs" instance as parameter
        /// Just call this method where you want to fire the event
        /// </summary>
        /// <param name="e"></param>
        public void RaiseProcessCompletedEvent(EventsArgs e)
        {
            //lock (syncLock)
            {
                if (processCompletedEvent != null)
                {
                    processCompletedEvent(this, e); //Fires the event
                }
            }
        }
        public event EventHandler processCompletedEvent;
    }


    public class EventsArgs : EventArgs
    {
        public string log { get; set; }

        public EventsArgs(string Log)
        {
            this.log = Log;
        }

        public Module module { get; set; }

        public EventsArgs(Module module)
        {
            this.module = module;
        }

        public EventsArgs()
        {
        }

        //public bool setTrueFalse { get; set; }

        //public EventsArgs(string Log, bool setTrueFalse)
        //{
        //    this.log = Log;
        //    this.setTrueFalse = setTrueFalse;
        //}

    }
}
