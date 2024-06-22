using Newtonsoft.Json;
using PMapCore.Common.Attrib;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRPCloud;


/// <summary>
/// Túraajánlat
/// </summary>
[Serializable]
public class PVRPCloudCalcTour
{
    public enum PVRCloudCalcTourStatus
    {
        [Description("OK")]
        OK,
        [Description("ERR")]
        ERR
    };
    public PVRPCloudCalcTour()
    {
        StatusEnum = PVRCloudCalcTourStatus.OK;
        Msg = new List<string>();

        Rank = 0;
        T1M = 0;
        T1Toll = 0;
        T1Cost = 0;
        T1Rest = 0;
        T1FullDuration = 0;
        T1Start = DateTime.MinValue;
        T1End = DateTime.MinValue;
        T1CalcRoute = new List<PVRPCloudCalcRoute>();

        RelM = 0;
        RelToll = 0;
        RelCost = 0;
        RelRest = 0;
        RelFullDuration = 0;
        RelStart = DateTime.MinValue;
        RelEnd = DateTime.MinValue;
        RelCalcRoute = new PVRPCloudCalcRoute();

        T2M = 0;
        T2Toll = 0;
        T2Cost = 0;
        T2Rest = 0;
        T2FullDuration = 0;
        T2Start = DateTime.MinValue;
        T2End = DateTime.MinValue;
        T2CalcRoute = new List<PVRPCloudCalcRoute>();

        RetM = 0;
        RetToll = 0;
        RetCost = 0;
        RetRest = 0;
        RetFullDuration = 0;
        RetStart = DateTime.MinValue;
        RetEnd = DateTime.MinValue;
        RetCalcRoute = new PVRPCloudCalcRoute();
    }

    //    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    [JsonIgnore]
    [IgnoreDataMember]
    public PVRCloudCalcTourStatus StatusEnum { get; set; }

    [DisplayNameAttributeX(Name = "Státusz", Order = 1)]
    public string Status { get { return StatusEnum.ToString(); } }

    [DisplayNameAttributeX(Name = "Üzenet", Order = 2)]
    public List<string> Msg { get; set; }

    [DisplayNameAttributeX(Name = "Helyezés", Order = 3)]
    public int Rank { get; set; }

    [DisplayNameAttributeX(Name = "Jármű", Order = 4)]
    public PVRPCloudTruck Truck { get; set; }

    #region túrarészletezők
    [DisplayNameAttributeX(Name = "I.túra hossz (m)", Order = 5)]
    public double T1M { get; set; }

    [DisplayNameAttributeX(Name = "I.túra útdíj", Order = 6)]
    public double T1Toll { get; set; }

    [DisplayNameAttributeX(Name = "I.túra költség", Order = 7)]
    public double T1Cost { get; set; }

    [DisplayNameAttributeX(Name = "I.túra pihenőidő", Order = 8)]
    public double T1Rest { get; set; }

    [DisplayNameAttributeX(Name = "I.túra időtartam", Order = 9)]
    public double T1FullDuration { get; set; }

    [DisplayNameAttributeX(Name = "I.túra kezdete", Order = 10)]
    public DateTime T1Start { get; set; }

    [DisplayNameAttributeX(Name = "I.túra vége", Order = 11)]
    public DateTime T1End { get; set; }

    [DisplayNameAttributeX(Name = "I.túra részletek", Order = 12)]
    public List<PVRPCloudCalcRoute> T1CalcRoute { get; set; }

    [DisplayNameAttributeX(Name = "Átállás hossz (m)", Order = 13)]
    public double RelM { get; set; }

    [DisplayNameAttributeX(Name = "Átállás útdíj", Order = 14)]
    public double RelToll { get; set; }

    [DisplayNameAttributeX(Name = "Átállás költség", Order = 15)]
    public double RelCost { get; set; }

    [DisplayNameAttributeX(Name = "Átállás pihenőidő", Order = 16)]
    public double RelRest { get; set; }

    [DisplayNameAttributeX(Name = "Átállás időtartam", Order = 17)]
    public double RelFullDuration { get; set; }

    [DisplayNameAttributeX(Name = "Átállás kezdete", Order = 18)]
    public DateTime RelStart { get; set; }

    [DisplayNameAttributeX(Name = "Átállás vége", Order = 19)]
    public DateTime RelEnd { get; set; }

    [DisplayNameAttributeX(Name = "Átállás részletek", Order = 20)]
    public PVRPCloudCalcRoute RelCalcRoute { get; set; }

    [DisplayNameAttributeX(Name = "II.túra hossz (m)", Order = 21)]
    public double T2M { get; set; }

    [DisplayNameAttributeX(Name = "II.túra útdíj", Order = 22)]
    public double T2Toll { get; set; }

    [DisplayNameAttributeX(Name = "II.túra költség", Order = 23)]
    public double T2Cost { get; set; }

    [DisplayNameAttributeX(Name = "II.túra pihenőidő", Order = 24)]
    public double T2Rest { get; set; }

    [DisplayNameAttributeX(Name = "II.túra időtartam", Order = 25)]
    public double T2FullDuration { get; set; }

    [DisplayNameAttributeX(Name = "II.túra kezdete", Order = 26)]
    public DateTime T2Start { get; set; }

    [DisplayNameAttributeX(Name = "II.túra vége", Order = 27)]
    public DateTime T2End { get; set; }

    [DisplayNameAttributeX(Name = "II.túra részletek", Order = 28)]
    public List<PVRPCloudCalcRoute> T2CalcRoute { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés hossz (m)", Order = 29)]
    public double RetM { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés  útdíj", Order = 30)]
    public double RetToll { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés  költség", Order = 31)]
    public double RetCost { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés puhenőidő", Order = 32)]
    public double RetRest { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés időtartam", Order = 33)]
    public double RetFullDuration { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés kezdete", Order = 34)]
    public DateTime RetStart { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés vége", Order = 35)]
    public DateTime RetEnd { get; set; }

    [DisplayNameAttributeX(Name = "Visszatérés részletek", Order = 36)]
    public PVRPCloudCalcRoute RetCalcRoute { get; set; }

    #endregion

    [DisplayNameAttributeX(Name = "Befejezés időpontja", Order = 37)]
    public DateTime TimeComplete { get { return Truck.CurrIsOneWay ? T2End : RetEnd; } }

    [DisplayNameAttributeX(Name = "Beosztás teljesítésének összköltsége", Order = 38)]
    public double AdditionalCost { get { return RelToll + RelCost + T2Toll + T2Cost + RetToll + RetCost; } }

    [DisplayNameAttributeX(Name = "Összes költség", Order = 39)]
    public double FullCost { get { return T1Toll + T1Cost + AdditionalCost; } }

    [DisplayNameAttributeX(Name = "Teljes hossz (m)", Order = 40)]
    public double FullM { get { return T1M + RelM + T2M + RetM; } }

    [DisplayNameAttributeX(Name = "Teljes időtartam", Order = 41)]
    public double FullDuration { get { return T1FullDuration + RelFullDuration + T2FullDuration + RetFullDuration; } }

}
