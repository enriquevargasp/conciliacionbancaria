CREATE PROCEDURE spJCREliminaEdoCuentaConciliacion
    @FolioExt INT ,
    @A�oExt INT ,
    @A�oConciliacion INT ,
    @MesConciliacion INT ,
    @FolioConciliacion INT
AS
    SET NOCOUNT ON; 

    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    DELETE  cp
    FROM    dbo.TablaDestino td
            JOIN dbo.TablaDestinoDetalle tdd ON tdd.Corporativo = td.Corporativo
                                                AND tdd.Sucursal = td.Sucursal
                                                AND tdd.A�o = td.A�o
                                                AND tdd.Folio = td.Folio
            JOIN dbo.ConciliacionPedido cp ON cp.CorporativoExterno = tdd.Corporativo
                                              AND cp.SucursalExterno = tdd.Sucursal
                                              AND cp.A�oExterno = tdd.A�o
                                              AND cp.FolioExterno = tdd.Folio
                                              AND cp.SecuenciaExterno = tdd.Secuencia
    WHERE   td.Folio = @FolioExt
            AND td.A�o = @A�oExt;



    DELETE  tdd
    FROM    dbo.TablaDestino td
            JOIN dbo.TablaDestinoDetalle tdd ON tdd.Corporativo = td.Corporativo
                                                AND tdd.Sucursal = td.Sucursal
                                                AND tdd.A�o = td.A�o
                                                AND tdd.Folio = td.Folio
    WHERE   td.Folio = @FolioExt
            AND td.A�o = @A�oExt;


    DELETE  FROM dbo.ConciliacionReferencia
    WHERE   FolioConciliacion = @FolioConciliacion
            AND MesConciliacion = @MesConciliacion
            AND A�oConciliacion = @A�oConciliacion;

    DELETE  FROM dbo.Conciliacion
    WHERE   FolioConciliacion = @FolioConciliacion
            AND MesConciliacion = @MesConciliacion
            AND A�oConciliacion = @A�oConciliacion;



    DELETE  tdd
    FROM    dbo.TablaDestino td
    WHERE   td.Folio = @FolioExt
            AND td.A�o = @A�oExt;