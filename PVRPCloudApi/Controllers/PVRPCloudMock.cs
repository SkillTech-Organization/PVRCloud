using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Controllers;

public static class PVRPCloudMock
{
    public static PVRPCloudResponse ResponseMock => new()
    {
        RequestID = "12345678",
        Results = [
            PVRPCloudResult.Success(Project)
        ]
    };

    private static PVRPCloudProjectRes Project => new()
    {
        ProjectName = "TEST1",
        MinTime = DateTime.Parse("2024.07.25 00:00:00"),
        MaxTime = DateTime.Parse("2024.07.25 23:59:59"),
        Tours = [
            new()
            {
                Truck = new(){
                ID =  "TRK1",
                TruckTypeID =  "TType1",
                TruckName =  "TRK-001",
                ArrDepotMaxTime =  1,
                CapacityProfileID =  "2T",
                MaxWorkTime =  1440,
                EarliestStart =  0,
                LatestStart =  1439
            },
            StartTime =  DateTime.Parse("2024.05.25 06:45:00"),
            EndTime =  DateTime.Parse("2024.05.25 11:05:00"),
            TourLength =  48370,
            //TourToll =  88.4,
            TourToll =  88,
            Route =  [
                new()
              {
                Lat =  47.623782,
                Lng =  19.120456
              },
                new()
              {
                Lat =  47.615889,
                Lng =  19.109627
              },
                new()
              {
                Lat =  47.615514,
                Lng =  19.109391
              },
                new()
              {
                Lat =  47.614104,
                Lng =  19.108501
              },
                new()
              {
                Lat =  47.613704,
                Lng =  19.108204
              },
                new()
              {
                Lat =  47.61147,
                Lng =  19.104932
              },
                new()
              {
                Lat =  47.611014,
                Lng =  19.10387
              },
                new()
              {
                Lat =  47.609392,
                Lng =  19.100729
              },
                new()
              {
                Lat =  47.606614,
                Lng =  19.098015
              },
                new()
              {
                Lat =  47.604622,
                Lng =  19.096131
              },
                new()
              {
                Lat =  47.596649,
                Lng =  19.090312
              },
                new()
              {
                Lat =  47.595357,
                Lng =  19.091308
              },
                new()
              {
                Lat =  47.595046,
                Lng =  19.094911
              },
                new()
              {
                Lat =  47.601157,
                Lng =  19.101017
              },
                new()
              {
                Lat =  47.603239,
                Lng =  19.100601
              },
                new()
              {
                Lat =  47.604167,
                Lng =  19.098667
              },
                new()
              {
                Lat =  47.612456,
                Lng =  19.07854
              },
                new()
              {
                Lat =  47.612835,
                Lng =  19.076898
              },
                new()
              {
                Lat =  47.613006,
                Lng =  19.074416
              },
                new()
              {
                Lat =  47.612592,
                Lng =  19.071422
              },
                new()
              {
                Lat =  47.612208,
                Lng =  19.070181
              },
                new()
              {
                Lat =  47.612018,
                Lng =  19.069644
              },
                new()
              {
                Lat =  47.612167,
                Lng =  19.066333
              },
                new()
              {
                Lat =  47.612256,
                Lng =  19.06581
              },
                new()
              {
                Lat =  47.612305,
                Lng =  19.065407
              },
                new()
              {
                Lat =  47.612334,
                Lng =  19.065131
              },
                new()
              {
                Lat =  47.612534,
                Lng =  19.065026
              },
                new()
              {
                Lat =  47.612588,
                Lng =  19.064464
              },
                new()
              {
                Lat =  47.612415,
                Lng =  19.064281
              },
                new()
              {
                Lat =  47.612198,
                Lng =  19.064336
              },
                new()
              {
                Lat =  47.610023,
                Lng =  19.062788
              },
                new()
              {
                Lat =  47.609035,
                Lng =  19.062263
              },
                new()
              {
                Lat =  47.606159,
                Lng =  19.060625
              },
                new()
              {
                Lat =  47.605153,
                Lng =  19.059936
              },
                new()
              {
                Lat =  47.604802,
                Lng =  19.059723
              },
                new()
              {
                Lat =  47.60234,
                Lng =  19.05809
              },
                new()
              {
                Lat =  47.595323,
                Lng =  19.053431
              },
                new()
              {
                Lat =  47.594159,
                Lng =  19.052633
              },
                new()
              {
                Lat =  47.591765,
                Lng =  19.051228
              },
                new()
              {
                Lat =  47.591191,
                Lng =  19.050948
              },
                new()
              {
                Lat =  47.589915,
                Lng =  19.050506
              },
                new()
              {
                Lat =  47.587644,
                Lng =  19.050057
              },
                new()
              {
                Lat =  47.587102,
                Lng =  19.049973
              },
                new()
              {
                Lat =  47.586608,
                Lng =  19.049917
              },
                new()
              {
                Lat =  47.585611,
                Lng =  19.049721
              },
                new()
              {
                Lat =  47.584619,
                Lng =  19.049575
              },
                new()
              {
                Lat =  47.584062,
                Lng =  19.049512
              },
                new()
              {
                Lat =  47.583603,
                Lng =  19.049465
              },
                new()
              {
                Lat =  47.583297,
                Lng =  19.049434
              },
                new()
              {
                Lat =  47.582307,
                Lng =  19.049335
              },
                new()
              {
                Lat =  47.580058,
                Lng =  19.049216
              },
                new()
              {
                Lat =  47.578655,
                Lng =  19.049127
              },
                new()
              {
                Lat =  47.578323,
                Lng =  19.049099
              },
                new()
              {
                Lat =  47.576297,
                Lng =  19.048927
              },
                new()
              {
                Lat =  47.574964,
                Lng =  19.04874
              },
                new()
              {
                Lat =  47.573296,
                Lng =  19.048438
              },
                new()
              {
                Lat =  47.568555,
                Lng =  19.048688
              },
                new()
              {
                Lat =  47.568224,
                Lng =  19.048808
              },
                new()
              {
                Lat =  47.567208,
                Lng =  19.049229
              },
                new()
              {
                Lat =  47.566691,
                Lng =  19.049344
              },
                new()
              {
                Lat =  47.566203,
                Lng =  19.049365
              },
              new(){ Lat =  47.565817,
                Lng =  19.04925
              },
              new(){ Lat =  47.561775,
                Lng =  19.047856
              },
              new(){
                Lat =  47.561127,
                Lng =  19.047635
              },
              new(){
                Lat =  47.557663,
                Lng =  19.046215
              },
              new(){
                Lat =  47.556677,
                Lng =  19.045784
              },
              new(){
                Lat =  47.555672,
                Lng =  19.04541
              },
              new(){
                Lat =  47.555278,
                Lng =  19.04525
              },
              new(){
                Lat =  47.551246,
                Lng =  19.044101
              },
              new(){
                Lat =  47.549744,
                Lng =  19.043683
              },
              new(){
                Lat =  47.549277,
                Lng =  19.043589
              },
              new(){
                Lat =  47.54557,
                Lng =  19.042653
              },
              new(){
                Lat =  47.544507,
                Lng =  19.042403
              },
              new(){
                Lat =  47.543602,
                Lng =  19.042172
              },
              new(){
                Lat =  47.54238,
                Lng =  19.041504
              },
              new(){
                Lat =  47.541086,
                Lng =  19.041001
              },
              new(){
                Lat =  47.540375,
                Lng =  19.040842
              },
              new(){
                Lat =  47.539658,
                Lng =  19.040752
              },
              new(){
                Lat =  47.538926,
                Lng =  19.04055
              },
              new(){
                Lat =  47.536853,
                Lng =  19.039918
              },
              new(){
                Lat =  47.534755,
                Lng =  19.039845
              },
              new(){
                Lat =  47.533488,
                Lng =  19.039835
              },
              new(){
                Lat =  47.532344,
                Lng =  19.039836
              },
              new(){
                Lat =  47.531401,
                Lng =  19.040035
              },
              new(){
                Lat =  47.531207,
                Lng =  19.039916
              },
              new(){
                Lat =  47.531026,
                Lng =  19.039828
              },
              new(){
                Lat =  47.530765,
                Lng =  19.039705
              },
              new(){
                Lat =  47.529402,
                Lng =  19.039191
              },
              new(){
                Lat =  47.528222,
                Lng =  19.038723
              },
              new(){
                Lat =  47.527486,
                Lng =  19.038453
              },
              new(){
                Lat =  47.526605,
                Lng =  19.038235
              },
              new(){
                Lat =  47.525229,
                Lng =  19.037902
              },
              new(){
                Lat =  47.524419,
                Lng =  19.037709
              },
              new(){
                Lat =  47.524507,
                Lng =  19.036929
              },
              new(){
                Lat =  47.52413,
                Lng =  19.036901
              },
              new(){
                Lat =  47.523918,
                Lng =  19.036897
              },
              new(){
                Lat =  47.523932,
                Lng =  19.036572
              },
              new(){
                Lat =  47.523865,
                Lng =  19.034676
              },
              new(){
                Lat =  47.522985,
                Lng =  19.03481
              },
              new(){
                Lat =  47.52308,
                Lng =  19.033679
              },
              new(){
                Lat =  47.522679,
                Lng =  19.033117
              },
              new(){
                Lat =  47.522599,
                Lng =  19.032327
              },
              new(){
                Lat =  47.52203,
                Lng =  19.030495
              },
              new(){
                Lat =  47.521805,
                Lng =  19.029861
              },
              new(){
                Lat =  47.520136,
                Lng =  19.0288
              },
              new(){
                Lat =  47.519312,
                Lng =  19.028507
              },
              new(){
                Lat =  47.518519,
                Lng =  19.028135
              },
              new(){
                Lat =  47.518272,
                Lng =  19.028042
              },
              new(){
                Lat =  47.517332,
                Lng =  19.027574
              },
              new(){
                Lat =  47.516983,
                Lng =  19.027429
              },
              new(){
                Lat =  47.515783,
                Lng =  19.026797
              },
              new(){
                Lat =  47.515702,
                Lng =  19.026318
              },
              new(){
                Lat =  47.516472,
                Lng =  19.024579
              },
              new(){
                Lat =  47.515935,
                Lng =  19.024129
              },
              new(){
                Lat =  47.515037,
                Lng =  19.023456
              },
              new(){
                Lat =  47.515126,
                Lng =  19.022515
              },
              new(){
                Lat =  47.51409,
                Lng =  19.021438
              },
              new(){
                Lat =  47.513471,
                Lng =  19.020326
              },
              new(){
                Lat =  47.514621,
                Lng =  19.018879
              },
              new(){
                Lat =  47.514377,
                Lng =  19.018359
              },
              new(){
                Lat =  47.513619,
                Lng =  19.016956
              },
              new(){
                Lat =  47.513351,
                Lng =  19.016477
              },
              new(){
                Lat =  47.513047,
                Lng =  19.016696
              },
              new(){
                Lat =  47.512581,
                Lng =  19.015961
              },
              new(){
                Lat =  47.512316,
                Lng =  19.015191
              },
              new(){
                Lat =  47.511695,
                Lng =  19.014567
              },
              new(){
                Lat =  47.510563,
                Lng =  19.013671
              },
              new(){
                Lat =  47.510701,
                Lng =  19.013202
              },
              new(){
                Lat =  47.510338,
                Lng =  19.012915
              },
              new(){
                Lat =  47.509747,
                Lng =  19.012569
              },
              new(){
                Lat =  47.509868,
                Lng =  19.011937
              },
              new(){
                Lat =  47.509894,
                Lng =  19.009209
              },
              new(){
                Lat =  47.510031,
                Lng =  19.00866
              },
              new(){
                Lat =  47.510034,
                Lng =  19.007958
              },
              new(){
                Lat =  47.511315,
                Lng =  19.001563
              },
              new(){
                Lat =  47.510098,
                Lng =  18.99965
              },
              new(){
                Lat =  47.509985,
                Lng =  18.998976
              },
              new(){
                Lat =  47.509405,
                Lng =  18.996821
              },
              new(){
                Lat =  47.509397,
                Lng =  18.996561
              },
              new(){
                Lat =  47.509107,
                Lng =  18.995197
              },
              new(){
                Lat =  47.508692,
                Lng =  18.995552
              },
              new(){
                Lat =  47.508031,
                Lng =  18.993245
              },
              new(){
                Lat =  47.506837,
                Lng =  18.990226
              },
              new(){
                Lat =  47.506957,
                Lng =  18.989212
              },
              new(){
                Lat =  47.506952,
                Lng =  18.987859
              },
              new(){
                Lat =  47.506002,
                Lng =  18.986492
              },
              new(){
                Lat =  47.504811,
                Lng =  18.986218
              },
              new(){
                Lat =  47.504811,
                Lng =  18.986218
              },
              new(){
                Lat =  47.506002,
                Lng =  18.986492
              },
              new(){
                Lat =  47.506952,
                Lng =  18.987859
              },
              new()
            {
                Lat =  47.506957,
                Lng =  18.989212
              },
              new(){
                Lat =  47.506837,
                Lng =  18.990226
              },
              new(){
                Lat =  47.508031,
                Lng =  18.993245
              },
              new(){
                Lat =  47.508692,
                Lng =  18.995552
              },
              new(){
                Lat =  47.509107,
                Lng =  18.995197
              },
              new(){
                Lat =  47.509397,
                Lng =  18.996561
              },
              new(){
                Lat =  47.509405,
                Lng =  18.996821
              },
              new(){
                Lat =  47.509985,
                Lng =  18.998976
              },
              new(){
                Lat =  47.510098,
                Lng =  18.99965
              },
              new(){
                Lat =  47.511315,
                Lng =  19.001563
              },
              new(){
                Lat =  47.510034,
                Lng =  19.007958
              },
              new(){
                Lat =  47.510031,
                Lng =  19.00866
              },
              new(){
                Lat =  47.509894,
                Lng =  19.009209
              },
              new(){
                Lat =  47.509868,
                Lng =  19.011937
              },
              new(){
                Lat =  47.509747,
                Lng =  19.012569
              },
              new(){
                Lat =  47.510338,
                Lng =  19.012915
              },
              new(){
                Lat =  47.510701,
                Lng =  19.013202
              },
              new(){
                Lat =  47.510563,
                Lng =  19.013671
              },
              new(){
                Lat =  47.511695,
                Lng =  19.014567
              },
              new(){
                Lat =  47.512316,
                Lng =  19.015191
              },
              new(){
                Lat =  47.512581,
                Lng =  19.015961
              },
              new(){
                Lat =  47.513047,
                Lng =  19.016696
              },
              new(){
                Lat =  47.513351,
                Lng =  19.016477
              },
              new(){
                Lat =  47.514838,
                Lng =  19.01501
              },
              new(){
                Lat =  47.516003,
                Lng =  19.016997
              },
              new(){
                Lat =  47.516818,
                Lng =  19.016223
              },
              new(){
                Lat =  47.517194,
                Lng =  19.016658
              },
              new(){
                Lat =  47.517901,
                Lng =  19.01732
              },
              new(){
                Lat =  47.517759,
                Lng =  19.017858
              },
              new(){
                Lat =  47.518261,
                Lng =  19.020976
              },
              new(){
                Lat =  47.519288,
                Lng =  19.021683
              },
              new(){
                Lat =  47.519785,
                Lng =  19.02208
              },
              new(){
                Lat =  47.520667,
                Lng =  19.023484
              },
              new(){
                Lat =  47.520638,
                Lng =  19.025055
              },
              new(){
                Lat =  47.520565,
                Lng =  19.025513
              },
              new(){
                Lat =  47.520442,
                Lng =  19.026938
              },
              new(){
                Lat =  47.520136,
                Lng =  19.0288
              },
              new(){
                Lat =  47.521805,
                Lng =  19.029861
              },
              new(){
                Lat =  47.52203,
                Lng =  19.030495
              },
              new(){
                Lat =  47.522599,
                Lng =  19.032327
              },
              new(){
                Lat =  47.522679,
                Lng =  19.033117
              },
              new(){
                Lat =  47.52308,
                Lng =  19.033679
              },
              new(){
                Lat =  47.522985,
                Lng =  19.03481
              },
              new(){
                Lat =  47.523865,
                Lng =  19.034676
              },
              new(){
                Lat =  47.523932,
                Lng =  19.036572
              },
              new(){
                Lat =  47.523918,
                Lng =  19.036897
              },
              new(){
                Lat =  47.52413,
                Lng =  19.036901
              },
              new(){
                Lat =  47.524507,
                Lng =  19.036929
              },
              new(){
                Lat =  47.524419,
                Lng =  19.037709
              },
              new(){
                Lat =  47.525229,
                Lng =  19.037902
              },
              new(){
                Lat =  47.526605,
                Lng =  19.038235
              },
              new(){
                Lat =  47.527486,
                Lng =  19.038453
              },
              new(){
                Lat =  47.528222,
                Lng =  19.038723
              },
              new(){
                Lat =  47.529402,
                Lng =  19.039191
              },
              new(){
                Lat =  47.530765,
                Lng =  19.039705
              },
              new(){
                Lat =  47.531026,
                Lng =  19.039828
              },
              new(){
                Lat =  47.531207,
                Lng =  19.039916
              },
              new(){
                Lat =  47.531401,
                Lng =  19.040035
              },
              new(){
                Lat =  47.532344,
                Lng =  19.039836
              },
              new(){
                Lat =  47.533488,
                Lng =  19.039835
              },
              new(){
                Lat =  47.534755,
                Lng =  19.039845
              },
              new(){
                Lat =  47.536853,
                Lng =  19.039918
              },
              new(){
                Lat =  47.538926,
                Lng =  19.04055
              },
              new(){
                Lat =  47.539658,
                Lng =  19.040752
              },
              new(){
                Lat =  47.540375,
                Lng =  19.040842
              },
              new(){
                Lat =  47.541086,
                Lng =  19.041001
              },
              new(){
                Lat =  47.54238,
                Lng =  19.041504
              },
              new(){
                Lat =  47.543602,
                Lng =  19.042172
              },
              new(){
                Lat =  47.544507,
                Lng =  19.042403
              },
              new(){
                Lat =  47.54557,
                Lng =  19.042653
              },
              new(){
                Lat =  47.549277,
                Lng =  19.043589
              },
              new(){
                Lat =  47.549744,
                Lng =  19.043683
              },
              new(){
                Lat =  47.551246,
                Lng =  19.044101
              },
              new(){
                Lat =  47.555278,
                Lng =  19.04525
              },
              new(){
                Lat =  47.555672,
                Lng =  19.04541
              },
              new(){
                Lat =  47.556677,
                Lng =  19.045784
              },
              new(){
                Lat =  47.557663,
                Lng =  19.046215
              },
              new(){
                Lat =  47.561127,
                Lng =  19.047635
              },
              new(){
                Lat =  47.561775,
                Lng =  19.047856
              },
              new(){
                Lat =  47.565817,
                Lng =  19.04925
              },
              new(){
                Lat =  47.566203,
                Lng =  19.049365
              },
              new(){
                Lat =  47.566691,
                Lng =  19.049344
              },
              new(){
                Lat =  47.567208,
                Lng =  19.049229
              },
              new(){
                Lat =  47.568224,
                Lng =  19.048808
              },
              new(){
                Lat =  47.568555,
                Lng =  19.048688
              },
              new(){
                Lat =  47.573296,
                Lng =  19.048438
              },
              new(){
                Lat =  47.574964,
                Lng =  19.04874
              },
              new(){
                Lat =  47.576297,
                Lng =  19.048927
              },
              new(){
                Lat =  47.578323,
                Lng =  19.049099
              },
              new(){
                Lat =  47.578655,
                Lng =  19.049127
              },
              new(){
                Lat =  47.580058,
                Lng =  19.049216
              },
              new(){
                Lat =  47.582307,
                Lng =  19.049335
              },
              new(){
                Lat =  47.583297,
                Lng =  19.049434
              },
              new(){
                Lat =  47.583603,
                Lng =  19.049465
              },
              new(){
                Lat =  47.584062,
                Lng =  19.049512
              },
              new(){
                Lat =  47.584619,
                Lng =  19.049575
              },
              new(){
                Lat =  47.585611,
                Lng =  19.049721
              },
              new(){
                Lat =  47.586608,
                Lng =  19.049917
              },
              new(){
                Lat =  47.587102,
                Lng =  19.049973
              },
              new(){
                Lat =  47.587644,
                Lng =  19.050057
              },
              new(){
                Lat =  47.589915,
                Lng =  19.050506
              },
              new(){
                Lat =  47.591191,
                Lng =  19.050948
              },
              new(){
                Lat =  47.591765,
                Lng =  19.051228
              },
              new(){
                Lat =  47.594159,
                Lng =  19.052633
              },
              new(){
                Lat =  47.595323,
                Lng =  19.053431
              },
              new(){
                Lat =  47.60234,
                Lng =  19.05809
              },
              new(){
                Lat =  47.604802,
                Lng =  19.059723
              },
              new(){
                Lat =  47.605153,
                Lng =  19.059936
              },
              new(){
                Lat =  47.606159,
                Lng =  19.060625
              },
              new(){
                Lat =  47.609035,
                Lng =  19.062263
              },
              new(){
                Lat =  47.609824,
                Lng =  19.062828
              },
              new(){
                Lat =  47.611527,
                Lng =  19.064746
              },
              new(){
                Lat =  47.612167,
                Lng =  19.066333
              },
              new(){
                Lat =  47.612018,
                Lng =  19.069644
              },
              new(){
                Lat =  47.612208,
                Lng =  19.070181
              },
              new(){
                Lat =  47.612592,
                Lng =  19.071422
              },
              new(){
                Lat =  47.613006,
                Lng =  19.074416
              },
              new(){
                Lat =  47.612835,
                Lng =  19.076898
              },
              new(){
                Lat =  47.612456,
                Lng =  19.07854
              },
              new(){
                Lat =  47.604167,
                Lng =  19.098667
              },
              new(){
                Lat =  47.603239,
                Lng =  19.100601
              },
              new(){
                Lat =  47.601157,
                Lng =  19.101017
              },
              new(){
                Lat =  47.595046,
                Lng =  19.094911
              },
              new(){
                Lat =  47.595357,
                Lng =  19.091308
              },
              new(){
                Lat =  47.596649,
                Lng =  19.090312
              },
              new(){
                Lat =  47.604622,
                Lng =  19.096131
              },
              new(){
                Lat =  47.606614,
                Lng =  19.098015
              },
              new(){
                Lat =  47.609392,
                Lng =  19.100729
              },
              new(){
                Lat =  47.611014,
                Lng =  19.10387
              },
              new(){
                Lat =  47.61147,
                Lng =  19.104932
              },
              new(){
                Lat =  47.613704,
                Lng =  19.108204
              },
              new(){
                Lat =  47.614104,
                Lng =  19.108501
              },
              new(){
                Lat =  47.615514,
                Lng =  19.109391
              },
              new(){
                Lat =  47.615889,
                Lng =  19.109627
              },
              new(){
                Lat =  47.623782,
                Lng =  19.120456
              }
            ],
            TourPoints =  [
                new()
              {
                Depot =  new(){
                  ID =  "WH1",
                  DepotName =  "Warehouse",
                  Lat =  47.623782,
                  Lng =  19.120456,
                  DepotMinTime =  240,
                  DepotMaxTime =  1380,
                  ServiceFixTime =  1,
                  ServiceVarTime =  0
                },
                Client =  null,
                Lat =  47.623782,
                Lng =  19.120456,
                TourPointNo = 1,
                Distance = 0,
                Duration = 0,
                ArrTime = DateTime.Parse("2024.05.25 06:45:00"),
                ServTime = DateTime.Parse("2024.05.25 06:45:00"),
                DepTime = DateTime.Parse("2024.05.25 06:46:00"),
                Order = null
              },
                new()
              {
                Depot =  null,
                Client = new()
                {
                  ID =  "Cl01",
                  ClientName = "Clent01",
                  Lat =  47.504811,
                  Lng =  18.986218,
                  ServiceFixTime = 10
                },
                Lat =  47.504811,
                Lng =  18.986218,
                TourPointNo = 1,
                Distance = 24200,
                Duration = 74,
                ArrTime = DateTime.Parse("2024.07.25 08:00:00"),
                ServTime = DateTime.Parse("2024.07.25 08:00:00"),
                DepTime = DateTime.Parse("2024.07.25 09:50:00"),
                Order = new(){
                  ID =  "ord1",
                  OrderName = "Order1",
                  ClientID = "Cl01",
                  Quantity1 = 1000,
                  Quantity2 = 0,
                  ReadyTime = 0,
                  TruckList = [
                    "TRK1",
                    "TRK2",
                    "TRK3"
                  ],
                  OrderServiceTime = 10,
                  OrderMinTime = 480,
                  OrderMaxTime = 960
                }
              },
                new()
              {
                Depot = new() {
                  ID =  "WH1",
                  DepotName =  "Warehouse",
                  Lat =  47.623782,
                  Lng =  19.120456,
                  DepotMinTime =  240,
                  DepotMaxTime =  1380,
                  ServiceFixTime =  1,
                  ServiceVarTime =  0
                },
                Client =  null,
                Lat =  47.623782,
                Lng =  19.120456,
                TourPointNo = 3,
                Distance = 24170,
                Duration = 74,
                ArrTime = DateTime.Parse("2024.05.25 11:04:00"),
                ServTime = DateTime.Parse("2024.05.25 11:04:00"),
                DepTime = DateTime.Parse("2024.05.25 11:04:00"),
                Order = null
              }
            ]
          },
          new()
          {
            Truck = new() {
              ID =  "TRK2",
              TruckTypeID =  "TType2",
              TruckName =  "TRK-002",
              ArrDepotMaxTime =  1,
              CapacityProfileID =  "7.5T",
              MaxWorkTime =  1440,
              EarliestStart =  0,
              LatestStart =  1439
            },
            StartTime =  DateTime.Parse("2024.07.25 04:52:00"),
            EndTime =  DateTime.Parse("2024.07.25 23:26:00"),
            TourLength =  396000,
            //TourToll =  16969.4,
            TourToll =  16969,
            Route =  [
              new(){
                Lat =  47.623782,
                Lng =  19.120456
              },
              new(){
                Lat =  47.615889,
                Lng =  19.109627
              },
              new(){
                Lat =  47.615514,
                Lng =  19.109391
              },
              new(){
                Lat =  47.614104,
                Lng =  19.108501
              },
              new(){
                Lat =  47.613704,
                Lng =  19.108204
              },
              new(){
                Lat =  47.61147,
                Lng =  19.104932
              },
              new(){
                Lat =  47.611014,
                Lng =  19.10387
              },
              new(){
                Lat =  47.609392,
                Lng =  19.100729
              },
              new(){
                Lat =  47.606614,
                Lng =  19.098015
              },
              new(){
                Lat =  47.604622,
                Lng =  19.096131
              },
              new(){
                Lat =  47.596649,
                Lng =  19.090312
              },
              new(){
                Lat =  47.595357,
                Lng =  19.091308
              },
              new(){
                Lat =  47.595046,
                Lng =  19.094911
              },
              new(){
                Lat =  47.601157,
                Lng =  19.101017
              },
              new(){
                Lat =  47.602136,
                Lng =  19.103082
              },
              new(){
                Lat =  47.602392,
                Lng =  19.1043
              },
              new(){
                Lat =  47.603995,
                Lng =  19.11683
              },
              new(){
                Lat =  47.603001,
                Lng =  19.119048
              },
              new(){
                Lat =  47.602678,
                Lng =  19.119557
              },
              new(){
                Lat =  47.601846,
                Lng =  19.120867
              },
              new(){
                Lat =  47.600665,
                Lng =  19.122621
              },
              new(){
                Lat =  47.599919,
                Lng =  19.124109
              },
              new(){
                Lat =  47.598431,
                Lng =  19.129446
              },
              new(){
                Lat =  47.597903,
                Lng =  19.13182
              },
              new(){
                Lat =  47.597251,
                Lng =  19.133852
              },
              new(){
                Lat =  47.596692,
                Lng =  19.13511
              },
              new(){
                Lat =  47.596044,
                Lng =  19.136293
              },
              new(){
                Lat =  47.594184,
                Lng =  19.138544
              },
              new(){
                Lat =  47.586249,
                Lng =  19.144437
              },
              new(){
                Lat =  47.584361,
                Lng =  19.145896
              },
              new(){
                Lat =  47.583675,
                Lng =  19.14644
              },
              new(){
                Lat =  47.581297,
                Lng =  19.148777
              },
              new(){
                Lat =  47.580828,
                Lng =  19.149355
              },
              new(){
                Lat =  47.580732,
                Lng =  19.149474
              },
              new(){
                Lat =  47.580651,
                Lng =  19.149574
              },
              new(){
                Lat =  47.578241,
                Lng =  19.152552
              },
              new(){
                Lat =  47.576394,
                Lng =  19.154894
              },
              new(){
                Lat =  47.573878,
                Lng =  19.158061
              },
              new(){
                Lat =  47.573332,
                Lng =  19.158751
              },
              new(){
                Lat =  47.573,
                Lng =  19.15917
              },
              new(){
                Lat =  47.572115,
                Lng =  19.160287
              },
              new(){
                Lat =  47.571722,
                Lng =  19.160783
              },
              new(){
                Lat =  47.570054,
                Lng =  19.162837
              },
              new(){
                Lat =  47.553936,
                Lng =  19.179335
              },
              new(){
                Lat =  47.548818,
                Lng =  19.188841
              },
              new(){
                Lat =  47.533605,
                Lng =  19.23563
              },
              new(){
                Lat =  47.533952,
                Lng =  19.236125
              },
              new(){
                Lat =  47.535209,
                Lng =  19.244914
              },
              new(){
                Lat =  47.533376,
                Lng =  19.247249
              },
              new(){
                Lat =  47.533075,
                Lng =  19.247507
              },
              new(){
                Lat =  47.53119,
                Lng =  19.249154
              },
              new(){
                Lat =  47.52988,
                Lng =  19.250149
              },
              new(){
                Lat =  47.520593,
                Lng =  19.257194
              },
              new(){
                Lat =  47.519711,
                Lng =  19.257778
              },
              new(){
                Lat =  47.516353,
                Lng =  19.260391
              },
              new(){
                Lat =  47.509521,
                Lng =  19.278577
              },
              new(){
                Lat =  47.509461,
                Lng =  19.279773
              },
              new(){
                Lat =  47.509329,
                Lng =  19.283628
              },
              new(){
                Lat =  47.509311,
                Lng =  19.284144
              },
              new(){
                Lat =  47.509223,
                Lng =  19.286071
              },
              new(){
                Lat =  47.509143,
                Lng =  19.288051
              },
              new(){
                Lat =  47.509071,
                Lng =  19.289845
              },
              new(){
                Lat =  47.508664,
                Lng =  19.295559
              },
              new(){
                Lat =  47.508536,
                Lng =  19.296445
              },
              new(){
                Lat =  47.508222,
                Lng =  19.297987
              },
              new(){
                Lat =  47.507499,
                Lng =  19.300272
              },
              new(){
                Lat =  47.505699,
                Lng =  19.303969
              },
              new(){
                Lat =  47.50445,
                Lng =  19.305607
              },
              new(){
                Lat =  47.493136,
                Lng =  19.310395
              },
              new(){
                Lat =  47.490532,
                Lng =  19.310911
              },
              new(){
                Lat =  47.486955,
                Lng =  19.312555
              },
              new(){
                Lat =  47.467791,
                Lng =  19.332181
              },
              new(){
                Lat =  47.464753,
                Lng =  19.331988
              },
              new(){
                Lat =  47.461339,
                Lng =  19.330488
              },
              new(){
                Lat =  47.460974,
                Lng =  19.330314
              },
              new(){
                Lat =  47.433836,
                Lng =  19.328202
              },
              new(){
                Lat =  47.430941,
                Lng =  19.32652
              },
              new(){
                Lat =  47.42882,
                Lng =  19.324896
              },
              new(){
                Lat =  47.413308,
                Lng =  19.315821
              },
              new(){
                Lat =  47.410775,
                Lng =  19.316034
              },
              new(){
                Lat =  47.404242,
                Lng =  19.314825
              },
              new(){
                Lat =  47.399667,
                Lng =  19.310091
              },
              new(){
                Lat =  47.385216,
                Lng =  19.284492
              },
              new(){
                Lat =  47.382835,
                Lng =  19.279307
              },
              new(){
                Lat =  47.377851,
                Lng =  19.265442
              },
              new(){
                Lat =  47.377095,
                Lng =  19.263185
              },
              new(){
                Lat =  47.3762,
                Lng =  19.260535
              },
              new(){
                Lat =  47.367983,
                Lng =  19.23634
              },
              new(){
                Lat =  47.367784,
                Lng =  19.23581
              },
              new(){
                Lat =  47.365961,
                Lng =  19.23071
              },
              new(){
                Lat =  47.356316,
                Lng =  19.209058
              },
              new(){
                Lat =  47.354515,
                Lng =  19.204085
              },
              new(){
                Lat =  47.353945,
                Lng =  19.200422
              },
              new(){
                Lat =  47.35479,
                Lng =  19.200544
              },
              new(){
                Lat =  47.353417,
                Lng =  19.201746
              },
              new(){
                Lat =  47.350233,
                Lng =  19.204793
              },
              new(){
                Lat =  47.348623,
                Lng =  19.206582
              },
              new(){
                Lat =  47.307443,
                Lng =  19.277129
              },
              new(){
                Lat =  47.305746,
                Lng =  19.279893
              },
              new(){
                Lat =  47.305456,
                Lng =  19.280362
              },
              new(){
                Lat =  47.277566,
                Lng =  19.321731
              },
              new(){
                Lat =  47.274227,
                Lng =  19.327429
              },
              new(){
                Lat =  47.271318,
                Lng =  19.332745
              },
              new(){
                Lat =  47.248777,
                Lng =  19.371172
              },
              new(){
                Lat =  47.246391,
                Lng =  19.374533
              },
              new(){
                Lat =  47.243659,
                Lng =  19.378368
              },
              new(){
                Lat =  47.224255,
                Lng =  19.401858
              },
              new(){
                Lat =  47.223522,
                Lng =  19.402547
              },
              new(){
                Lat =  47.220852,
                Lng =  19.405121
              },
              new(){
                Lat =  47.21955,
                Lng =  19.406359
              },
              new(){
                Lat =  47.217097,
                Lng =  19.408489
              },
              new(){
                Lat =  47.150816,
                Lng =  19.460054
              },
              new(){
                Lat =  47.148786,
                Lng =  19.462109
              },
              new(){
                Lat =  47.148306,
                Lng =  19.462596
              },
              new(){
                Lat =  47.140213,
                Lng =  19.471794
              },
              new(){
                Lat =  47.137992,
                Lng =  19.474617
              },
              new(){
                Lat =  47.079173,
                Lng =  19.529682
              },
              new(){
                Lat =  47.044197,
                Lng =  19.561995
              },
              new(){
                Lat =  47.039399,
                Lng =  19.567889
              },
              new(){
                Lat =  47.039278,
                Lng =  19.568038
              },
              new(){
                Lat =  46.990109,
                Lng =  19.609379
              },
              new(){
                Lat =  46.989889,
                Lng =  19.609432
              },
              new(){
                Lat =  46.987704,
                Lng =  19.609959
              },
              new(){
                Lat =  46.935318,
                Lng =  19.618188
              },
              new(){
                Lat =  46.933588,
                Lng =  19.617678
              },
              new(){
                Lat =  46.932926,
                Lng =  19.617477
              },
              new(){
                Lat =  46.925354,
                Lng =  19.615372
              },
              new(){
                Lat =  46.92264,
                Lng =  19.614818
              },
              new(){
                Lat =  46.887548,
                Lng =  19.62686
              },
              new(){
                Lat =  46.887447,
                Lng =  19.626952
              },
              new(){
                Lat =  46.885109,
                Lng =  19.629073
              },
              new(){
                Lat =  46.849647,
                Lng =  19.66253
              },
              new(){
                Lat =  46.849496,
                Lng =  19.662755
              },
              new(){
                Lat =  46.845978,
                Lng =  19.668233
              },
              new(){
                Lat =  46.7212,
                Lng =  19.79258
              },
              new(){
                Lat =  46.720915,
                Lng =  19.792713
              },
              new(){
                Lat =  46.718338,
                Lng =  19.793918
              },
              new(){
                Lat =  46.678879,
                Lng =  19.822025
              },
              new(){
                Lat =  46.676981,
                Lng =  19.823497
              },
              new(){
                Lat =  46.676333,
                Lng =  19.824
              },
              new(){
                Lat =  46.572617,
                Lng =  19.874211
              },
              new(){
                Lat =  46.464633,
                Lng =  19.927973
              },
              new(){
                Lat =  46.463926,
                Lng =  19.928282
              },
              new(){
                Lat =  46.461811,
                Lng =  19.929185
              },
              new(){
                Lat =  46.404352,
                Lng =  19.971206
              },
              new(){
                Lat =  46.308,
                Lng =  20.041167
              },
              new(){
                Lat =  46.303018,
                Lng =  20.042589
              },
              new(){
                Lat =  46.301914,
                Lng =  20.045088
              },
              new(){
                Lat =  46.299021,
                Lng =  20.053029
              },
              new(){
                Lat =  46.293081,
                Lng =  20.077496
              },
              new(){
                Lat =  46.292988,
                Lng =  20.078798
              },
              new(){
                Lat =  46.292969,
                Lng =  20.08218
              },
              new(){
                Lat =  46.292849,
                Lng =  20.082511
              },
              new(){
                Lat =  46.292124,
                Lng =  20.083586
              },
              new(){
                Lat =  46.290713,
                Lng =  20.085205
              },
              new(){
                Lat =  46.290617,
                Lng =  20.085307
              },
              new(){
                Lat =  46.285744,
                Lng =  20.089396
              },
              new(){
                Lat =  46.283575,
                Lng =  20.091285
              },
              new(){
                Lat =  46.283291,
                Lng =  20.091504
              },
              new(){
                Lat =  46.282356,
                Lng =  20.092209
              },
              new(){
                Lat =  46.282168,
                Lng =  20.092188
              },
              new(){
                Lat =  46.282032,
                Lng =  20.092354
              },
              new(){
                Lat =  46.282008,
                Lng =  20.092587
              },
              new(){
                Lat =  46.28148,
                Lng =  20.093404
              },
              new(){
                Lat =  46.280788,
                Lng =  20.094191
              },
              new(){
                Lat =  46.280207,
                Lng =  20.094817
              },
              new(){
                Lat =  46.279598,
                Lng =  20.095431
              },
              new(){
                Lat =  46.278927,
                Lng =  20.096104
              },
              new(){
                Lat =  46.277452,
                Lng =  20.097594
              },
              new(){
                Lat =  46.277235,
                Lng =  20.097782
              },
              new(){
                Lat =  46.277019,
                Lng =  20.097969
              },
              new(){
                Lat =  46.276194,
                Lng =  20.098754
              },
              new(){
                Lat =  46.275551,
                Lng =  20.099321
              },
              new(){
                Lat =  46.27477,
                Lng =  20.100015
              },
              new(){
                Lat =  46.274658,
                Lng =  20.100115
              },
              new(){
                Lat =  46.274349,
                Lng =  20.100388
              },
              new(){
                Lat =  46.272696,
                Lng =  20.101793
              },
              new(){
                Lat =  46.272376,
                Lng =  20.101991
              },
              new(){
                Lat =  46.271835,
                Lng =  20.102243
              },
              new(){
                Lat =  46.271518,
                Lng =  20.102391
              },
              new(){
                Lat =  46.270058,
                Lng =  20.103024
              },
              new(){
                Lat =  46.268449,
                Lng =  20.103729
              },
              new(){
                Lat =  46.267995,
                Lng =  20.103885
              },
              new(){
                Lat =  46.267432,
                Lng =  20.104414
              },
              new(){
                Lat =  46.267318,
                Lng =  20.105195
              },
              new(){
                Lat =  46.266794,
                Lng =  20.108271
              },
              new(){
                Lat =  46.2667,
                Lng =  20.108723
              },
              new(){
                Lat =  46.266653,
                Lng =  20.108904
              },
              new(){
                Lat =  46.266314,
                Lng =  20.11048
              },
              new(){
                Lat =  46.26624,
                Lng =  20.110763
              },
              new(){
                Lat =  46.265335,
                Lng =  20.110305
              },
              new(){
                Lat =  46.265335,
                Lng =  20.110305
              },
              new(){
                Lat =  46.26624,
                Lng =  20.110763
              },
              new(){
                Lat =  46.266314,
                Lng =  20.11048
              },
              new(){
                Lat =  46.266653,
                Lng =  20.108904
              },
              new(){
                Lat =  46.2667,
                Lng =  20.108723
              },
              new(){
                Lat =  46.266794,
                Lng =  20.108271
              },
              new(){
                Lat =  46.267318,
                Lng =  20.105195
              },
              new(){
                Lat =  46.267383,
                Lng =  20.105062
              },
              new(){
                Lat =  46.267888,
                Lng =  20.104146
              },
              new(){
                Lat =  46.268449,
                Lng =  20.103729
              },
              new(){
                Lat =  46.270058,
                Lng =  20.103024
              },
              new(){
                Lat =  46.271518,
                Lng =  20.102391
              },
              new(){
                Lat =  46.271835,
                Lng =  20.102243
              },
              new(){
                Lat =  46.272376,
                Lng =  20.101991
              },
              new(){
                Lat =  46.272696,
                Lng =  20.101793
              },
              new(){
                Lat =  46.274349,
                Lng =  20.100388
              },
              new(){
                Lat =  46.274658,
                Lng =  20.100115
              },
              new(){
                Lat =  46.27477,
                Lng =  20.100015
              },
              new(){
                Lat =  46.275551,
                Lng =  20.099321
              },
              new(){
                Lat =  46.276194,
                Lng =  20.098754
              },
              new(){
                Lat =  46.277019,
                Lng =  20.097969
              },
              new(){
                Lat =  46.277235,
                Lng =  20.097782
              },
              new(){
                Lat =  46.277452,
                Lng =  20.097594
              },
              new(){
                Lat =  46.278927,
                Lng =  20.096104
              },
              new(){
                Lat =  46.279598,
                Lng =  20.095431
              },
              new(){
                Lat =  46.280207,
                Lng =  20.094817
              },
              new(){
                Lat =  46.280788,
                Lng =  20.094191
              },
              new(){
                Lat =  46.28148,
                Lng =  20.093404
              },
              new(){
                Lat =  46.282139,
                Lng =  20.092833
              },
              new(){
                Lat =  46.282485,
                Lng =  20.092496
              },
              new(){
                Lat =  46.283291,
                Lng =  20.091504
              },
              new(){
                Lat =  46.283575,
                Lng =  20.091285
              },
              new(){
                Lat =  46.285744,
                Lng =  20.089396
              },
              new(){
                Lat =  46.290617,
                Lng =  20.085307
              },
              new(){
                Lat =  46.290713,
                Lng =  20.085205
              },
              new(){
                Lat =  46.292124,
                Lng =  20.083586
              },
              new(){
                Lat =  46.293002,
                Lng =  20.08281
              },
              new(){
                Lat =  46.293278,
                Lng =  20.082595
              },
              new(){
                Lat =  46.293435,
                Lng =  20.082436
              },
              new(){
                Lat =  46.293828,
                Lng =  20.082374
              },
              new(){
                Lat =  46.294145,
                Lng =  20.082414
              },
              new(){
                Lat =  46.294222,
                Lng =  20.082208
              },
              new(){
                Lat =  46.29415,
                Lng =  20.081929
              },
              new(){
                Lat =  46.29392,
                Lng =  20.081867
              },
              new(){
                Lat =  46.293215,
                Lng =  20.077448
              },
              new(){
                Lat =  46.298674,
                Lng =  20.056473
              },
              new(){
                Lat =  46.299197,
                Lng =  20.053251
              },
              new(){
                Lat =  46.302173,
                Lng =  20.045181
              },
              new(){
                Lat =  46.305,
                Lng =  20.042333
              },
              new(){
                Lat =  46.404441,
                Lng =  19.971394
              },
              new(){
                Lat =  46.459249,
                Lng =  19.930417
              },
              new(){
                Lat =  46.4635,
                Lng =  19.928667
              },
              new(){
                Lat =  46.464676,
                Lng =  19.928183
              },
              new(){
                Lat =  46.57271,
                Lng =  19.874433
              },
              new(){
                Lat =  46.67647,
                Lng =  19.824213
              },
              new(){
                Lat =  46.67691,
                Lng =  19.823861
              },
              new(){
                Lat =  46.680394,
                Lng =  19.821074
              },
              new(){
                Lat =  46.717898,
                Lng =  19.794385
              },
              new(){
                Lat =  46.721041,
                Lng =  19.792874
              },
              new(){
                Lat =  46.721241,
                Lng =  19.79278
              },
              new(){
                Lat =  46.845597,
                Lng =  19.669325
              },
              new(){
                Lat =  46.846755,
                Lng =  19.667914
              },
              new(){
                Lat =  46.847278,
                Lng =  19.667714
              },
              new(){
                Lat =  46.849719,
                Lng =  19.665361
              },
              new(){
                Lat =  46.850616,
                Lng =  19.664022
              },
              new(){
                Lat =  46.853052,
                Lng =  19.66751
              },
              new(){
                Lat =  46.870823,
                Lng =  19.68603
              },
              new(){
                Lat =  46.877964,
                Lng =  19.680208
              },
              new(){
                Lat =  46.87896,
                Lng =  19.679921
              },
              new(){
                Lat =  46.881153,
                Lng =  19.680632
              },
              new(){
                Lat =  46.882642,
                Lng =  19.681697
              },
              new(){
                Lat =  46.883604,
                Lng =  19.682201
              },
              new(){
                Lat =  46.883704,
                Lng =  19.68233
              },
              new(){
                Lat =  46.884063,
                Lng =  19.682561
              },
              new(){
                Lat =  46.884606,
                Lng =  19.682862
              },
              new(){
                Lat =  46.885127,
                Lng =  19.683091
              },
              new(){
                Lat =  46.885234,
                Lng =  19.683119
              },
              new(){
                Lat =  46.885863,
                Lng =  19.683401
              },
              new(){
                Lat =  46.8865,
                Lng =  19.683671
              },
              new(){
                Lat =  46.887173,
                Lng =  19.683955
              },
              new(){
                Lat =  46.88772,
                Lng =  19.684185
              },
              new(){
                Lat =  46.888331,
                Lng =  19.684454
              },
              new(){
                Lat =  46.888932,
                Lng =  19.684724
              },
              new(){
                Lat =  46.889578,
                Lng =  19.684994
              },
              new(){
                Lat =  46.890197,
                Lng =  19.685264
              },
              new(){
                Lat =  46.890825,
                Lng =  19.685547
              },
              new(){
                Lat =  46.891418,
                Lng =  19.685777
              },
              new(){
                Lat =  46.894237,
                Lng =  19.68686
              },
              new(){
                Lat =  46.894981,
                Lng =  19.687238
              },
              new(){
                Lat =  46.897996,
                Lng =  19.688598
              },
              new(){
                Lat =  46.898831,
                Lng =  19.688963
              },
              new(){
                Lat =  46.899303,
                Lng =  19.689182
              },
              new(){
                Lat =  46.899433,
                Lng =  19.689153
              },
              new(){
                Lat =  46.901268,
                Lng =  19.685947
              },
              new(){
                Lat =  46.901404,
                Lng =  19.685778
              },
              new(){
                Lat =  46.902111,
                Lng =  19.684815
              },
              new(){
                Lat =  46.902758,
                Lng =  19.68419
              },
              new(){
                Lat =  46.903531,
                Lng =  19.683388
              },
              new(){
                Lat =  46.903594,
                Lng =  19.683323
              },
              new(){
                Lat =  46.904056,
                Lng =  19.682856
              },
              new(){
                Lat =  46.90468,
                Lng =  19.682194
              },
              new(){
                Lat =  46.904985,
                Lng =  19.682092
              },
              new(){
                Lat =  46.905264,
                Lng =  19.682293
              },
              new(){
                Lat =  46.905721,
                Lng =  19.682679
              },
              new(){
                Lat =  46.905891,
                Lng =  19.682812
              },
              new(){
                Lat =  46.90633,
                Lng =  19.683145
              },
              new(){
                Lat =  46.906908,
                Lng =  19.683556
              },
              new(){
                Lat =  46.908125,
                Lng =  19.683928
              },
              new(){
                Lat =  46.908586,
                Lng =  19.684122
              },
              new(){
                Lat =  46.909515,
                Lng =  19.684732
              },
              new(){
                Lat =  46.910286,
                Lng =  19.685253
              },
              new(){
                Lat =  46.91094,
                Lng =  19.68572
              },
              new(){
                Lat =  46.91125,
                Lng =  19.685908
              },
              new(){
                Lat =  46.911818,
                Lng =  19.686309
              },
              new(){
                Lat =  46.913102,
                Lng =  19.686849
              },
              new(){
                Lat =  46.914413,
                Lng =  19.687219
              },
              new(){
                Lat =  46.914869,
                Lng =  19.68729
              },
              new(){
                Lat =  46.915412,
                Lng =  19.687404
              },
              new(){
                Lat =  46.915506,
                Lng =  19.687422
              },
              new(){
                Lat =  46.915921,
                Lng =  19.687534
              },
              new(){
                Lat =  46.916137,
                Lng =  19.687615
              },
              new(){
                Lat =  46.916452,
                Lng =  19.687742
              },
              new(){
                Lat =  46.916847,
                Lng =  19.687865
              },
              new(){
                Lat =  46.917323,
                Lng =  19.688042
              },
              new(){
                Lat =  46.917826,
                Lng =  19.688218
              },
              new(){
                Lat =  46.918136,
                Lng =  19.688652
              },
              new(){
                Lat =  46.918197,
                Lng =  19.689365
              },
              new(){
                Lat =  46.918036,
                Lng =  19.689681
              },
              new(){
                Lat =  46.917132,
                Lng =  19.691904
              },
              new(){
                Lat =  46.916702,
                Lng =  19.692931
              },
              new(){
                Lat =  46.916291,
                Lng =  19.693937
              },
              new(){
                Lat =  46.915963,
                Lng =  19.694721
              },
              new(){
                Lat =  46.915126,
                Lng =  19.696418
              },
              new(){
                Lat =  46.914891,
                Lng =  19.696612
              },
              new(){
                Lat =  46.914403,
                Lng =  19.697039
              },
              new(){
                Lat =  46.913164,
                Lng =  19.698101
              },
              new(){
                Lat =  46.913046,
                Lng =  19.698217
              },
              new(){
                Lat =  46.91198,
                Lng =  19.699136
              },
              new(){
                Lat =  46.911187,
                Lng =  19.699826
              },
              new(){
                Lat =  46.911092,
                Lng =  19.69994
              },
              new(){
                Lat =  46.908704,
                Lng =  19.702178
              },
              new(){
                Lat =  46.908774,
                Lng =  19.702292
              },
              new(){
                Lat =  46.910525,
                Lng =  19.705
              },
              new(){
                Lat =  46.910917,
                Lng =  19.705595
              },
              new(){
                Lat =  46.911024,
                Lng =  19.705754
              },
              new(){
                Lat =  46.91122,
                Lng =  19.706085
              },
              new(){
                Lat =  46.911648,
                Lng =  19.706812
              },
              new(){
                Lat =  46.911755,
                Lng =  19.706997
              },
              new(){
                Lat =  46.912013,
                Lng =  19.707473
              },
              new(){
                Lat =  46.912801,
                Lng =  19.71108
              },
              new(){
                Lat =  46.912809,
                Lng =  19.711408
              },
              new(){
                Lat =  46.912051,
                Lng =  19.717818
              },
              new(){
                Lat =  46.911572,
                Lng =  19.720209
              },
              new(){
                Lat =  46.911635,
                Lng =  19.720387
              },
              new(){
                Lat =  46.911892,
                Lng =  19.720507
              },
              new(){
                Lat =  46.912881,
                Lng =  19.720507
              },
              new(){
                Lat =  46.913493,
                Lng =  19.72209
              },
              new(){
                Lat =  46.913493,
                Lng =  19.72209
              },
              new(){
                Lat =  46.912881,
                Lng =  19.720507
              },
              new(){
                Lat =  46.911892,
                Lng =  19.720507
              },
              new(){
                Lat =  46.911635,
                Lng =  19.720387
              },
              new(){
                Lat =  46.911572,
                Lng =  19.720209
              },
              new(){
                Lat =  46.912051,
                Lng =  19.717818
              },
              new(){
                Lat =  46.912809,
                Lng =  19.711408
              },
              new(){
                Lat =  46.912801,
                Lng =  19.71108
              },
              new(){
                Lat =  46.912013,
                Lng =  19.707473
              },
              new(){
                Lat =  46.911755,
                Lng =  19.706997
              },
              new(){
                Lat =  46.911648,
                Lng =  19.706812
              },
              new(){
                Lat =  46.91122,
                Lng =  19.706085
              },
              new(){
                Lat =  46.911024,
                Lng =  19.705754
              },
              new(){
                Lat =  46.910917,
                Lng =  19.705595
              },
              new(){
                Lat =  46.910525,
                Lng =  19.705
              },
              new(){
                Lat =  46.908774,
                Lng =  19.702292
              },
              new(){
                Lat =  46.908867,
                Lng =  19.702236
              },
              new(){
                Lat =  46.911142,
                Lng =  19.700107
              },
              new(){
                Lat =  46.91121,
                Lng =  19.70002
              },
              new(){
                Lat =  46.911319,
                Lng =  19.699903
              },
              new(){
                Lat =  46.912033,
                Lng =  19.699295
              },
              new(){
                Lat =  46.91309,
                Lng =  19.698375
              },
              new(){
                Lat =  46.913289,
                Lng =  19.698194
              },
              new(){
                Lat =  46.914519,
                Lng =  19.697145
              },
              new(){
                Lat =  46.914953,
                Lng =  19.69677
              },
              new(){
                Lat =  46.915259,
                Lng =  19.696486
              },
              new(){
                Lat =  46.916207,
                Lng =  19.694514
              },
              new(){
                Lat =  46.916399,
                Lng =  19.694057
              },
              new(){
                Lat =  46.916818,
                Lng =  19.693038
              },
              new(){
                Lat =  46.917242,
                Lng =  19.691993
              },
              new(){
                Lat =  46.918323,
                Lng =  19.689432
              },
              new(){
                Lat =  46.918229,
                Lng =  19.688499
              },
              new(){
                Lat =  46.917916,
                Lng =  19.688101
              },
              new(){
                Lat =  46.917351,
                Lng =  19.687884
              },
              new(){
                Lat =  46.916866,
                Lng =  19.687708
              },
              new(){
                Lat =  46.916462,
                Lng =  19.687585
              },
              new(){
                Lat =  46.916162,
                Lng =  19.687474
              },
              new(){
                Lat =  46.916047,
                Lng =  19.687427
              },
              new(){
                Lat =  46.915837,
                Lng =  19.686777
              },
              new(){
                Lat =  46.916015,
                Lng =  19.686503
              },
              new(){
                Lat =  46.917008,
                Lng =  19.6849
              },
              new(){
                Lat =  46.918461,
                Lng =  19.682512
              },
              new(){
                Lat =  46.920266,
                Lng =  19.679674
              },
              new(){
                Lat =  46.921009,
                Lng =  19.678471
              },
              new(){
                Lat =  46.922109,
                Lng =  19.676688
              },
              new(){
                Lat =  46.922681,
                Lng =  19.67575
              },
              new(){
                Lat =  46.923552,
                Lng =  19.674355
              },
              new(){
                Lat =  46.923607,
                Lng =  19.674251
              },
              new(){
                Lat =  46.924006,
                Lng =  19.673599
              },
              new(){
                Lat =  46.924686,
                Lng =  19.672488
              },
              new(){
                Lat =  46.924705,
                Lng =  19.672337
              },
              new(){
                Lat =  46.925353,
                Lng =  19.670831
              },
              new(){
                Lat =  46.925771,
                Lng =  19.669694
              },
              new(){
                Lat =  46.927433,
                Lng =  19.665891
              },
              new(){
                Lat =  46.928969,
                Lng =  19.664397
              },
              new(){
                Lat =  46.929381,
                Lng =  19.664138
              },
              new(){
                Lat =  46.931154,
                Lng =  19.663454
              },
              new(){
                Lat =  46.938306,
                Lng =  19.660731
              },
              new(){
                Lat =  46.939266,
                Lng =  19.659947
              },
              new(){
                Lat =  46.939734,
                Lng =  19.65953
              },
              new(){
                Lat =  46.939978,
                Lng =  19.659312
              },
              new(){
                Lat =  46.940162,
                Lng =  19.659148
              },
              new(){
                Lat =  46.941122,
                Lng =  19.658268
              },
              new(){
                Lat =  46.942549,
                Lng =  19.65695
              },
              new(){
                Lat =  46.942913,
                Lng =  19.656555
              },
              new(){
                Lat =  46.954049,
                Lng =  19.64843
              },
              new(){
                Lat =  46.962649999999996,
                Lng =  19.642062
              },
              new(){
                Lat =  46.964022,
                Lng =  19.640865
              },
              new(){
                Lat =  46.965232,
                Lng =  19.639788
              },
              new(){
                Lat =  46.987805,
                Lng =  19.613439
              },
              new(){
                Lat =  46.995831,
                Lng =  19.607912
              },
              new(){
                Lat =  47.039504,
                Lng =  19.568117
              },
              new(){
                Lat =  47.039682,
                Lng =  19.567894
              },
              new(){
                Lat =  47.043677,
                Lng =  19.562947
              },
              new(){
                Lat =  47.081536,
                Lng =  19.528176
              },
              new(){
                Lat =  47.137537,
                Lng =  19.475629
              },
              new(){
                Lat =  47.140044,
                Lng =  19.472428
              },
              new(){
                Lat =  47.148419,
                Lng =  19.462843
              },
              new(){
                Lat =  47.148875,
                Lng =  19.462364
              },
              new(){
                Lat =  47.150866,
                Lng =  19.460328
              },
              new(){
                Lat =  47.216777,
                Lng =  19.409049
              },
              new(){
                Lat =  47.219614,
                Lng =  19.406654
              },
              new(){
                Lat =  47.221036,
                Lng =  19.405259
              },
              new(){
                Lat =  47.223568,
                Lng =  19.402819
              },
              new(){
                Lat =  47.224318,
                Lng =  19.402139
              },
              new(){
                Lat =  47.243987,
                Lng =  19.378368
              },
              new(){
                Lat =  47.248777,
                Lng =  19.371618
              },
              new(){
                Lat =  47.271443,
                Lng =  19.332884
              },
              new(){
                Lat =  47.27224,
                Lng =  19.331428
              },
              new(){
                Lat =  47.276233,
                Lng =  19.324384
              },
              new(){
                Lat =  47.303293,
                Lng =  19.284019
              },
              new(){
                Lat =  47.305568,
                Lng =  19.280527
              },
              new(){
                Lat =  47.305819,
                Lng =  19.280133
              },
              new(){
                Lat =  47.34786,
                Lng =  19.207635
              },
              new(){
                Lat =  47.34943,
                Lng =  19.206267
              },
              new(){
                Lat =  47.35022,
                Lng =  19.205698
              },
              new(){
                Lat =  47.352683,
                Lng =  19.205662
              },
              new(){
                Lat =  47.355936,
                Lng =  19.209052
              },
              new(){
                Lat =  47.35752,
                Lng =  19.212242
              },
              new(){
                Lat =  47.366285,
                Lng =  19.232087
              },
              new(){
                Lat =  47.367848,
                Lng =  19.236578
              },
              new(){
                Lat =  47.367978,
                Lng =  19.23695
              },
              new(){
                Lat =  47.375902,
                Lng =  19.260224
              },
              new(){
                Lat =  47.376901,
                Lng =  19.263174
              },
              new(){
                Lat =  47.376968,
                Lng =  19.26337
              },
              new(){
                Lat =  47.377693,
                Lng =  19.265508
              },
              new(){
                Lat =  47.383472,
                Lng =  19.281198
              },
              new(){
                Lat =  47.385361,
                Lng =  19.285232
              },
              new(){
                Lat =  47.399213,
                Lng =  19.309614
              },
              new(){
                Lat =  47.404221,
                Lng =  19.315107
              },
              new(){
                Lat =  47.409806,
                Lng =  19.316501
              },
              new(){
                Lat =  47.413308,
                Lng =  19.315821
              },
              new(){
                Lat =  47.42882,
                Lng =  19.324896
              },
              new(){
                Lat =  47.430941,
                Lng =  19.32652
              },
              new(){
                Lat =  47.433836,
                Lng =  19.328202
              },
              new(){
                Lat =  47.460974,
                Lng =  19.330314
              },
              new(){
                Lat =  47.461339,
                Lng =  19.330488
              },
              new(){
                Lat =  47.464753,
                Lng =  19.331988
              },
              new(){
                Lat =  47.467791,
                Lng =  19.332181
              },
              new(){
                Lat =  47.486955,
                Lng =  19.312555
              },
              new(){
                Lat =  47.490532,
                Lng =  19.310911
              },
              new(){
                Lat =  47.493136,
                Lng =  19.310395
              },
              new(){
                Lat =  47.50445,
                Lng =  19.305607
              },
              new(){
                Lat =  47.505699,
                Lng =  19.303969
              },
              new(){
                Lat =  47.507499,
                Lng =  19.300272
              },
              new(){
                Lat =  47.508222,
                Lng =  19.297987
              },
              new(){
                Lat =  47.508536,
                Lng =  19.296445
              },
              new(){
                Lat =  47.508664,
                Lng =  19.295559
              },
              new(){
                Lat =  47.509071,
                Lng =  19.289845
              },
              new(){
                Lat =  47.509143,
                Lng =  19.288051
              },
              new(){
                Lat =  47.509223,
                Lng =  19.286071
              },
              new(){
                Lat =  47.509311,
                Lng =  19.284144
              },
              new(){
                Lat =  47.509329,
                Lng =  19.283628
              },
              new(){
                Lat =  47.509461,
                Lng =  19.279773
              },
              new(){
                Lat =  47.509521,
                Lng =  19.278577
              },
              new(){
                Lat =  47.516353,
                Lng =  19.260391
              },
              new(){
                Lat =  47.519711,
                Lng =  19.257778
              },
              new(){
                Lat =  47.520593,
                Lng =  19.257194
              },
              new(){
                Lat =  47.52988,
                Lng =  19.250149
              },
              new(){
                Lat =  47.53119,
                Lng =  19.249154
              },
              new(){
                Lat =  47.533075,
                Lng =  19.247507
              },
              new(){
                Lat =  47.533376,
                Lng =  19.247249
              },
              new(){
                Lat =  47.535209,
                Lng =  19.244914
              },
              new(){
                Lat =  47.533952,
                Lng =  19.236125
              },
              new(){
                Lat =  47.533605,
                Lng =  19.23563
              },
              new(){
                Lat =  47.548818,
                Lng =  19.188841
              },
              new(){
                Lat =  47.553936,
                Lng =  19.179335
              },
              new(){
                Lat =  47.570054,
                Lng =  19.162837
              },
              new(){
                Lat =  47.570372,
                Lng =  19.162606
              },
              new(){
                Lat =  47.571417,
                Lng =  19.161464
              },
              new(){
                Lat =  47.573658,
                Lng =  19.158738
              },
              new(){
                Lat =  47.574365,
                Lng =  19.157826
              },
              new(){
                Lat =  47.576338,
                Lng =  19.15537
              },
              new(){
                Lat =  47.576553,
                Lng =  19.155104
              },
              new(){
                Lat =  47.577858,
                Lng =  19.153451
              },
              new(){
                Lat =  47.577965,
                Lng =  19.153316
              },
              new(){
                Lat =  47.580545,
                Lng =  19.150089
              },
              new(){
                Lat =  47.58164,
                Lng =  19.148757
              },
              new(){
                Lat =  47.583389,
                Lng =  19.146978
              },
              new(){
                Lat =  47.586249,
                Lng =  19.144437
              },
              new(){
                Lat =  47.594184,
                Lng =  19.138544
              },
              new(){
                Lat =  47.596044,
                Lng =  19.136293
              },
              new(){
                Lat =  47.596692,
                Lng =  19.13511
              },
              new(){
                Lat =  47.597251,
                Lng =  19.133852
              },
              new(){
                Lat =  47.597903,
                Lng =  19.13182
              },
              new(){
                Lat =  47.598431,
                Lng =  19.129446
              },
              new(){
                Lat =  47.599919,
                Lng =  19.124109
              },
              new(){
                Lat =  47.600665,
                Lng =  19.122621
              },
              new(){
                Lat =  47.601846,
                Lng =  19.120867
              },
              new(){
                Lat =  47.602678,
                Lng =  19.119557
              },
              new(){
                Lat =  47.603001,
                Lng =  19.119048
              },
              new(){
                Lat =  47.603995,
                Lng =  19.11683
              },
              new(){
                Lat =  47.602392,
                Lng =  19.1043
              },
              new(){
                Lat =  47.602136,
                Lng =  19.103082
              },
              new(){
                Lat =  47.601157,
                Lng =  19.101017
              },
              new(){
                Lat =  47.595046,
                Lng =  19.094911
              },
              new(){
                Lat =  47.595357,
                Lng =  19.091308
              },
              new(){
                Lat =  47.596649,
                Lng =  19.090312
              },
              new(){
                Lat =  47.604622,
                Lng =  19.096131
              },
              new(){
                Lat =  47.606614,
                Lng =  19.098015
              },
              new(){
                Lat =  47.609392,
                Lng =  19.100729
              },
              new(){
                Lat =  47.611014,
                Lng =  19.10387
              },
              new(){
                Lat =  47.61147,
                Lng =  19.104932
              },
              new(){
                Lat =  47.613704,
                Lng =  19.108204
              },
              new(){
                Lat =  47.614104,
                Lng =  19.108501
              },
              new(){
                Lat =  47.615514,
                Lng =  19.109391
              },
              new(){
                Lat =  47.615889,
                Lng =  19.109627
              },
              new(){
                Lat =  47.623782,
                Lng =  19.120456
              }
            ],
            TourPoints =  [
                new()
              {
                Depot = new() {
                  ID =  "WH1",
                  DepotName =  "Warehouse",
                  Lat =  47.623782,
                  Lng =  19.120456,
                  DepotMinTime =  240,
                  DepotMaxTime =  1380,
                  ServiceFixTime =  1,
                  ServiceVarTime =  0
                },
                Client =  null,
                Lat =  47.623782,
                Lng =  19.120456,
                TourPointNo = 1,
                Distance = 0,
                Duration = 0,
                ArrTime = DateTime.Parse("2024.07.25 04:52:00"),
                ServTime = DateTime.Parse("2024.07.25 04:52:00"),
                DepTime = DateTime.Parse("2024.07.25 04:53:00"),
                Order = null
              },
              new()
              {
                Depot =  null,
                Client = new() {
                  ID =  "Cl03",
                  ClientName = "Clent03",
                  Lat =  46.265335,
                  Lng =  20.110305,
                  ServiceFixTime = 5
                },
                Lat =  46.265335,
                Lng =  20.110305,
                TourPointNo = 2,
                Distance = 193540,
                Duration = 187,
                ArrTime = DateTime.Parse("2024.07.25 08:00:00"),
                ServTime = DateTime.Parse("2024.07.25 08:00:00"),
                DepTime = DateTime.Parse("2024.07.25 13:05:00"),
                Order = new() {
                  ID =  "ord3",
                  OrderName = "Order3",
                  ClientID = "Cl03",
                  Quantity1 = 3000,
                  Quantity2 = 0,
                  ReadyTime = 0,
                  TruckList = [
                    "TRK1",
                    "TRK2",
                    "TRK3"
                  ],
                  OrderServiceTime = 10,
                  OrderMinTime = 480,
                  OrderMaxTime = 960
                }
              },
              new()
              {
                Depot =  null,
                Client = new(){
                  ID =  "Cl02",
                  ClientName = "Clent02",
                  Lat =  46.913493,
                  Lng =  19.722090,
                  ServiceFixTime = 15
                },
                Lat =  46.913493,
                Lng =  19.722090,
                TourPointNo = 3,
                Distance = 88250,
                Duration = 86,
                ArrTime = DateTime.Parse("2024.07.25 14:31:00"),
                ServTime = DateTime.Parse("2024.07.25 14:31:00"),
                DepTime = DateTime.Parse("2024.07.25 21:26:00"),
                Order = new(){
                  ID =  "ord2",
                  OrderName = "Order2",
                  ClientID = "Cl01",
                  Quantity1 = 2000,
                  Quantity2 = 0,
                  ReadyTime = 0,
                  TruckList = [
                    "TRK1",
                    "TRK2",
                    "TRK3"
                  ],
                  OrderServiceTime = 10,
                  OrderMinTime = 480,
                  OrderMaxTime = 960
                }
              },
              new()
              {
                Depot = new() {
                  ID =  "WH1",
                  DepotName =  "Warehouse",
                  Lat =  47.623782,
                  Lng =  19.120456,
                  DepotMinTime =  240,
                  DepotMaxTime =  1380,
                  ServiceFixTime =  1,
                  ServiceVarTime =  0
                },
                Client =  null,
                Lat =  47.623782,
                Lng =  19.120456,
                TourPointNo = 4,
                Distance = 11384,
                Duration = 120,
                ArrTime = DateTime.Parse("2024.07.25 23:26:00"),
                ServTime = DateTime.Parse("2024.07.25 23:26:00"),
                DepTime = DateTime.Parse("2024.07.25 23:26:00"),
                Order = null
              }
            ]
          }
        ],
        UnplannedOrders = [
            new()
          {
            Order = new() {
              ID =  "ord4",
              OrderName = "Order4",
              ClientID = "Cl03",
              Quantity1 = 1,
              Quantity2 = 0,
              ReadyTime = 0,
              TruckList = [],
              OrderServiceTime = 10,
              OrderMinTime = 480,
              OrderMaxTime = 960
            }
          }
        ]
    };
}
