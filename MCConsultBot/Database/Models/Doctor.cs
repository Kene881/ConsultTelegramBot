using System;
using System.Collections.Generic;
using System.Text;

namespace MCConsultBot.Database.Models
{
    //Doctors (id, first name, last name, id_room, id_sched)
    class Doctor
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int price { get; set; }
        public string schedule { get; set; }
        public int room { get; set; }

        public Doctor(string fname, string lname, int price, string schedule, int room)
        {
            first_name = fname;
            last_name = lname;
            this.price = price;
            this.schedule = schedule;
            this.room = room;
        }
    }
}
