/*
Procedimiento para dar de alta los datos de una cobranza.
Autor: Santiago Mendoza Carlos Nirari
Fecha: 11/07/2017
*/

CREATE PROCEDURE spCBGuardarPedidoCobranza
    @A�oPed SMALLINT ,
    @Celula TINYINT ,
    @Pedido INT ,
    @Cobranza INT ,
    @Saldo MONEY ,
    @GestionInicial TINYINT = 1
AS
    SET NOCOUNT ON


    INSERT  INTO PedidoCobranza
            ( A�oPed ,
              Celula ,
              Pedido ,
              Cobranza ,
              Saldo ,
              GestionInicial
            )
    VALUES  ( @A�oPed ,
              @Celula ,
              @Pedido ,
              @Cobranza ,
              @Saldo ,
              @GestionInicial
            )
