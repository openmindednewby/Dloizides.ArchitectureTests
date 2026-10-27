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

[Route("v1/reports")]
public abstract class ReportsControllerBase : ControllerBase;

public sealed class DailyReportsController : ReportsControllerBase;

public sealed class MonthlyReportsController : ReportsControllerBase;

[Route("v1")]
public sealed class MeController : ControllerBase;

[Route("v1")]
public sealed class QuotaController : ControllerBase;

[Route("/api/v1/")]
public sealed class BrandingController : ControllerBase;

[Route("api/v1")]
public sealed class WatchlistVersionController : ControllerBase;
