using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class SmartMeter
    {
        private string meterId;
        private string ownerName;
        private double energyConsumed;
        private string zone;

        public SmartMeter(string meterId, string ownerName, double energyConsumed,string zone){
            this.meterId = meterId;
            this.ownerName = ownerName;
            this.energyConsumed = energyConsumed;
            this.zone = zone;
        }

        [DataMember]
        public string MeterId { get => meterId; set => meterId = value; }
        [DataMember]
        public string OwnerName { get => ownerName; set => ownerName = value; }
        [DataMember]
        public double EnergyConsumed { get => energyConsumed; set => energyConsumed = value; }
        [DataMember]
        public string Zone { get => zone; set => zone = value; }

        public override string ToString()
        {
            return String.Format("Meter Id : {0}, owner name : {1}, energy consumed: {2}, zone: {3}", MeterId, OwnerName, EnergyConsumed, Zone);
        }




    }
}
