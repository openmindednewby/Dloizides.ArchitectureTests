using Microsoft.AspNetCore.Mvc;

namespace Mvc.Service;

[Route("/v1/Cases/")]
public sealed class CaseArchiveController : ControllerBase;

[Route("v1/ingest")]
public sealed class IngestController : ControllerBase;

[Route("v1/ingest")]
public sealed class IngestJobController : ControllerBase;

[Route("v1/audit")]
public sealed class AuditController : ControllerBase;
