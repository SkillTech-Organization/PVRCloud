using FluentAssertions;
using PMapCore.BO;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloud.Models;

namespace PVRPCloudApiTests.ProblemFile;

public class RelationsRendererTests
{
    private static readonly TruckType[] _truckTypes = [
        new()
        {
            ID = "truck type id",
            SpeedValues = new Dictionary<int, int>
            {
                [1] = 5,
            }
        }
    ];

    private static readonly Dictionary<string, int> _truckTypeIds = new()
    {
        ["truck type id"] = 1
    };

    private static readonly NodeCombination clientNodes = new(
        new ClientNodeIdPair(new Depot() { ID = "depot" }, 15),
        new ClientNodeIdPair(new Client() { ID = "client" }, 29)
    );

    private static readonly Dictionary<string, int> _clientIds = new()
    {
        ["depot"] = 1,
        ["client"] = 2
    };

    private readonly RelationsRenderer _sut = new(_truckTypes, _truckTypeIds, [clientNodes], _clientIds);

    [Fact]
    public void Render_CalledWithRoutes_GeneratesSetRelationAccessSections()
    {
        PMapRoute route = new()
        {
            fromNOD_ID = 15,
            toNOD_ID = 29,
            TruckTypeId = "truck type id",
            route = new boRoute()
            {
                DST_DISTANCE = 5,
                Edges = [
                    new boEdge()
                    {
                        EDG_LENGTH = 1000,
                        RDT_VALUE = 1
                    }
                ]
            }
        };

        var result = _sut.Render([route]);

        result.ToString().Should().Be($"setRelationAccess(1, 1, 2, 5, 12){Environment.NewLine}");
    }
}
