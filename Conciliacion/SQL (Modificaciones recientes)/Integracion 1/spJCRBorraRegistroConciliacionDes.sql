CREATE PROCEDURE spJCRBorraRegistroConciliacionDes  
  
  
  
 @CorporativoConciliacion INT ,  
    @SucursalConciliacion INT ,  
    @A�oconciliacion INT ,  
    @Mesconciliacion INT ,  
    @FolioConciliacion INT  
  
  
 AS  
 SET NOCOUNT ON   
   
      
  
SELECT  tdd.* INTO TmpConciliacionReferencia  
FROM    dbo.TablaDestinoDetalle cf  
        JOIN dbo.ConciliacionReferencia tdd ON tdd.CorporativoExterno = cf.Corporativo  
                                               AND tdd.SucursalExterno = cf.Sucursal  
                                               AND tdd.A�oExterno = cf.A�o  
                                               AND tdd.FolioExterno = cf.Folio  
                                               AND tdd.SecuenciaExterno = cf.Secuencia  
WHERE   tdd.CorporativoConciliacion = @CorporativoConciliacion  
        AND tdd.SucursalConciliacion = @SucursalConciliacion  
        AND tdd.A�oConciliacion = @A�oconciliacion  
        AND tdd.MesConciliacion = @Mesconciliacion  
        AND tdd.FolioConciliacion = @FolioConciliacion;       
  
  
DELETE tdd  
FROM    dbo.TablaDestinoDetalle cf  
        JOIN dbo.ConciliacionReferencia tdd ON tdd.CorporativoExterno = cf.Corporativo  
                                               AND tdd.SucursalExterno = cf.Sucursal  
                                               AND tdd.A�oExterno = cf.A�o  
                                               AND tdd.FolioExterno = cf.Folio  
                                               AND tdd.SecuenciaExterno = cf.Secuencia  
WHERE   tdd.CorporativoConciliacion = @CorporativoConciliacion  
        AND tdd.SucursalConciliacion = @SucursalConciliacion  
        AND tdd.A�oConciliacion = @A�oconciliacion  
        AND tdd.MesConciliacion = @Mesconciliacion  
        AND tdd.FolioConciliacion = @FolioConciliacion;       
  
  
  
  
UPDATE cf  
 SET cf.StatusConciliacion = 'EN PROCESO DE CONCILIACION',cf.MotivoNoConciliado=NULL    
FROM    dbo.TablaDestinoDetalle cf  
        JOIN dbo.TmpConciliacionReferencia tdd ON tdd.CorporativoExterno = cf.Corporativo  
                                               AND tdd.SucursalExterno = cf.Sucursal  
                                               AND tdd.A�oExterno = cf.A�o  
                                               AND tdd.FolioExterno = cf.Folio  
                                               AND tdd.SecuenciaExterno = cf.Secuencia  
WHERE   tdd.CorporativoConciliacion = @CorporativoConciliacion  
        AND tdd.SucursalConciliacion = @SucursalConciliacion  
        AND tdd.A�oConciliacion = @A�oconciliacion  
        AND tdd.MesConciliacion = @Mesconciliacion  
        AND tdd.FolioConciliacion = @FolioConciliacion;       
  
  
  DROP TABLE TmpConciliacionReferencia  
  
  
update dbo.Conciliacion SET statusconciliacion = 'CONCILIACION ABIERTA' WHERE  CorporativoConciliacion = @CorporativoConciliacion  
        AND SucursalConciliacion = @SucursalConciliacion  
        AND A�oConciliacion = @A�oconciliacion  
        AND MesConciliacion = @Mesconciliacion  
        AND FolioConciliacion = @FolioConciliacion; 