@ModelType CalendarioVacunacionJsonVM
@Code
    ViewData("Title") = "Calendario"
    Dim i As Integer = 0

End Code

<style>


    .referencias {
        width: 15px !important;
        height: 15px !important;
        border: 1px solid;
        float: left;
        margin-right: 2px;
    }

    .referenciaslabel {
        font-size: smaller;
        float: left;
    }

    th:first-child, td:first-child {
        position: sticky;
        left: 0px;
    }
</style>

@section MenuHC
    @Html.Action("_Navegacion", "HistoriaClinica", New With {.id = Model.paciCodigo})
End Section
@section DatosPaciente
    @Html.Action("_DatosPaciente", "HistoriaClinica", New With {.id = Model.paciCodigo})
End Section

<div class="row">
    <div class="col-6">
        <h2>Calendario de Vacunación</h2>
    </div>
    <div class="col-6">
        <a href="@Url.Action("Index", New With {.id = Model.paciCodigo})" class="btn btn-secondary btn-sm float-right"><span class="fa fa-chevron-circle-left fa-fw" aria-hidden="true"></span> Volver a la lista</a>
    </div>
</div>
@If ViewBag.turnCodigo > 0 Then
    @<a href="@Url.Action("FinalizarAtencion", "HistoriaClinica", New With {.id = Model.paciCodigo})" data-toggle="tooltip" data-placement="top" title="Finalizar Atención" class="btn btn-warning">
        <span class="fa fa-check-circle fa-fw" aria-hidden="true"></span> Finalizar
    </a>
End If
<div style="margin-top:0px;margin-bottom:5px" class="row">
    <div class="col-md-2">
        <div class="referencias" style="background-color:#ff6666"></div><label class="referenciaslabel">Vacuna no correspondiente</label>
    </div>
    <div class="col-md-2">
        <div class="referencias" style="background-color:#ffd800"></div><label class="referenciaslabel">Vacuna aplicada fuera de termino</label>
    </div>

    <div class="col-md-2">
        <div class="referencias" style="background-color:#00ff21"></div><label class="referenciaslabel">Vacuna disponible para aplicar</label>
    </div>
    <div class="col-md-2">
        <div class="referencias" style="background-color:#0094ff"></div><label class="referenciaslabel">Vacuna aplicada</label>
    </div>
</div>
<div Class="">
    <table Class="table table-sm table-striped table-hover table-bordered table-sm small">
        <thead>
            <tr>
                <th>
                    <b> Vacuna/Edad</b>
                </th>
                @For Each vac In Model.Vacunas.OrderBy(Function(s) s.vacuOrden)
                    @<th>@vac.vacuDescripcion</th>
                Next
            </tr>
        </thead>
        <tbody>
            @For Each ran In Model.Rangos.OrderBy(Function(s) s.vareOrden).Where(Function(s) s.vareCodigoInterno <> "UNI")
                @<tr>
                    <th>@ran.vareDescripcion</th>
                    @For Each vac In Model.Vacunas.OrderBy(Function(s) s.vacuOrden)
                        @<td style="padding:0px">
                            @For Each apl In Model.Aplicaciones.Where(Function(s) s.vacuCodigo = vac.vacuCodigo And s.vareCodigo = ran.vareCodigo)
                                @<div style="background-color:@apl.Color">
                                    @If apl.Aplicar Then
                                        @<label style="cursor: pointer" onclick="aplicacion(0, @apl.vacuCodigo, @apl.vareCodigo, @apl.vadoCodigo)">
                                            @apl.vadoDosis
                                        </label>
                                    ElseIf apl.pavaCodigo > 0 Then
                                        @<label class="col-md-10" style="cursor: pointer" onclick="aplicacion(@apl.pavaCodigo)">
                                            @apl.vadoDosis
                                        </label>
                                    ElseIf Not apl.Aplicar Then
                                        @<label Class="col-md-10">
                                            @apl.vadoDosis
                                        </label>
                                    End If
                                    @If Not IsNothing(apl.Observacion) And apl.Observacion.Length > 0 Then
                                        @<span  style="cursor:pointer" class="badge badge-dark" onclick="showObservacion('@Html.Raw(apl.Observacion)')"><span class="fa fa-w fa-info"></span> </span>
                                    End If
                                </div>

                            Next
                        </td>
                    Next
                </tr>
            Next
        </tbody>
    </table>
</div>

<div class="modal" tabindex="-1" role="dialog" id="modalObservacion">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Observación</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <p id="textObservacion"></p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>


@section scripts
    <script>

        function SinFecha(obj, modal) {
            $('.pavaLote').val('')
        }

        function showObservacion(obs) {
            $('#textObservacion').text(obs)
            $('#modalObservacion').modal('show')
        }

        function Registro(obj, modal) {
            var val = $(obj).val()
            if (val == 'RA') {
                $('.pavaSinFecha').addClass('collapse')
            } else {
                $('.pavaSinFecha').removeClass('collapse')
            }
            $('.dvRegistroAplicacion').removeClass('collapse')
            $('.btn-save-vacunacion').removeAttr('disabled')
        }

        function aplicacion(pavaCodigo, vacuCodigo, vareCodigo, vadoCodigo) {
            var url = '@Url.Action("_Aplicacion")'
            url = url + '?paciCodigo=' + @Model.paciCodigo;
            if (pavaCodigo > 0) url = url + '&pavaCodigo=' + pavaCodigo
            if (vacuCodigo!=undefined) url = url + '&vacuCodigo=' + vacuCodigo
            if (vareCodigo!= undefined) url = url + '&vareCodigo=' + vareCodigo
            if (vadoCodigo != undefined) url = url + '&vadoCodigo=' + vadoCodigo
            $('#dvTempGlobal').load(url, function () {
                $('#modalAplicacion').modal('show')
            })
        }

        function success(data) {
            if (data.state == 1) {
                window.location.href = '@Url.Action("CalendarioVacunacion", New With {.id = Model.paciCodigo})'
            } else {
                myApp.mostrarError(data.error, "Error")
            }
        }


    </script>
End Section


