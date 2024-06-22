using PVRCloudApi.DTO.Request;
using PVRPCloud;

namespace PVRCloudApi.Handlers;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
public interface IPVRCloudApiHandler
{

    /// <summary>
    /// Calculate by PVRCloudSupporter engine
    /// </summary>
    /// <param name="body"></param>
    /// <param name="content_Type"></param>
    /// <param name="accept"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PVRPCloudResponse> PVRCloudSupportAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get calculation result
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PVRPCloudResponse> Result(string id);

    /// <summary>
    /// Calculate by PVRCloudSupporterX engine
    /// </summary>
    /// <param name="body"></param>
    /// <param name="content_Type"></param>
    /// <param name="accept"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PVRPCloudResponse> PVRCloudSupportXAsync(PVRCloudSupportRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// get the 'isalive' status of the PVRCloudSupporter service
    /// </summary>
    /// <param name="accept"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IsAliveAsync(CancellationToken cancellationToken = default);

}
