CREATE PROCEDURE spJCREliminaPagoConciliacion
    @Clave VARCHAR(50) ,
    @CorporativoConciliacion INT ,
    @SucursalConciliacion INT ,
    @A�o INT ,
    @FolioConciliacion INT ,
    @Mes INT
AS
    SET NOCOUNT ON;

    UPDATE  ConciliacionPedido
    SET     StatusMovimiento = 'PENDIENTE'---STATUS ORIGINAL(APLICADO,PENDIENTE)
    WHERE   CorporativoConciliacion = @CorporativoConciliacion
            AND SucursalConciliacion = @SucursalConciliacion
            AND A�oConciliacion = @A�o
            AND FolioConciliacion = @FolioConciliacion
            AND MesConciliacion = @Mes;

    DECLARE @Consecutivo INT ,
        @Folio INT ,
        @Caja INT ,
        @FOperacion DATETIME;


    SELECT  @Consecutivo = Consecutivo ,
            @Folio = Folio ,
            @Caja = Caja ,
            @FOperacion = FOperacion
    FROM    dbo.MovimientoCaja
    WHERE   Clave = @Clave;


    EXEC spCBMovimientoCajaRollBack @Caja, @FOperacion, @Consecutivo, @Folio;