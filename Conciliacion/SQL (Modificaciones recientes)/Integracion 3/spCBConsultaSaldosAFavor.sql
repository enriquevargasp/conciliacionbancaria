CREATE PROCEDURE spCBConsultaSaldosAFavor
    @Todos BIT ,
    @FIni DATETIME ,
    @FFin DATETIME ,
    @StatusConciliacion VARCHAR(20) = NULL ,
    @Cliente INT = NULL ,
    @Monto INT = NULL
AS
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT  MAC.FolioMovimiento ,
            cl.Cliente ,
            cl.Nombre AS NombreCliente ,
            C.CuentaBancoFinanciero AS CuentaBancaria ,
            B.Nombre AS Banco ,
            TDD.SucursalBancaria ,
            TC.Descripcion AS TipoCargo ,
            f.FGlobal AS Global ,
            CO.FDeposito AS FSaldo ,
            CO.Saldo AS Importe ,
            MAC.StatusConciliacion AS Conciliada
    FROM    dbo.MovimientoAConciliar MAC
            JOIN dbo.Conciliacion C ON C.CorporativoConciliacion = MAC.CorporativoConciliacion
                                       AND C.SucursalConciliacion = MAC.SucursalConciliacion
                                       AND C.A�oConciliacion = MAC.A�oConciliacion
                                       AND C.MesConciliacion = MAC.MesConciliacion
                                       AND C.FolioConciliacion = MAC.FolioConciliacion
            JOIN dbo.TablaDestino TD ON TD.Corporativo = C.CorporativoExterno
                                        AND TD.Sucursal = C.SucursalExterno
                                        AND TD.A�o = C.A�oExterno
                                        AND TD.Folio = C.FolioExterno
            JOIN dbo.TablaDestinoDetalle TDD ON TDD.Corporativo = MAC.CorporativoExterno
                                                AND TDD.Sucursal = MAC.SucursalExterno
                                                AND TDD.A�o = MAC.A�oExterno
                                                AND TDD.Folio = MAC.FolioExterno
                                                AND TDD.Secuencia = MAC.SecuenciaExterno
            JOIN Cobro CO ON CO.A�oCobro = MAC.A�oCobro
                             AND CO.Cobro = MAC.Cobro
            JOIN Banco B ON B.Banco = CO.Banco
            JOIN dbo.CobroPedido CP ON CP.A�oCobro = CO.A�oCobro
                                       AND CP.Cobro = CO.Cobro
            JOIN dbo.Pedido P ON P.Celula = CP.Celula
                                 AND P.A�oPed = CP.A�oPed
                                 AND P.Pedido = CP.Pedido
            JOIN dbo.TipoCargo TC ON TC.TipoCargo = P.TipoCargo
            JOIN Cliente cl ON cl.Cliente = P.Cliente
            JOIN dbo.ConciliacionPedido CPE ON CPE.Celula = P.Celula
                                               AND CPE.A�oPed = P.A�oPed
                                               AND CPE.Pedido = P.Pedido
            LEFT JOIN dbo.FacturaPedido FP ON FP.Celula = P.Celula
                                              AND FP.A�oPed = P.A�oPed
                                              AND FP.Pedido = P.Pedido
            LEFT JOIN dbo.Factura f ON f.Factura = FP.Factura
    WHERE   CASE WHEN @Todos = 0
                      AND CO.FDeposito BETWEEN @FIni AND @FFin THEN 1
                 WHEN @Todos = 1 THEN 1
                 ELSE 0
            END = 1
            AND ( @StatusConciliacion IS NULL
                  OR @StatusConciliacion = ''
                  OR @StatusConciliacion = 'TODAS'
                  OR MAC.StatusConciliacion = @StatusConciliacion
                )
            AND @Cliente IS NULL
            OR cl.Cliente = @Cliente
            AND @Monto IS NULL
            OR MAC.Monto = @Monto
            --AND CO.SaldoAFavor = 1
            --AND CO.Saldo > 0;

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    SET NOCOUNT OFF;	