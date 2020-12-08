function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_referencia').val(dados.Referencia);

    $('#ddl_negocio').val(dados.TipoNegocio);
    $('#ddl_tipoimovel').val(dados.TipoImovel);
    $('#txt_preco').val(dados.Preco);
    $('#txt_dormitorio').val(dados.Dormitorio);
    $('#txt_vagas').val(dados.Vagas);
    $('#txt_area').val(dados.Area);
    $('#cbx_ativo').val(dados.Ativo);
    $('#cbx_permuta').val(dados.Permuta);
    $('#txt_descricao').val(dados.Descricao);

    $('#ddl_estado').val(dados.Estado);

    $('#ddl_cidade').val(dados.Cidade);
    $('#ddl_cidade').prop('disabled', dados.Cidade <= 0 || dados.Cidade == undefined);
    $('#ddl_regiao').val(dados.Regiao);
    $('#ddl_regiao').prop('disabled', dados.Regiao <= 0 || dados.Regiao == undefined);
    $('#ddl_bairro').val(dados.Bairro);
    $('#ddl_bairro').prop('disabled', dados.Bairro <= 0 || dados.Bairro == undefined);

}

function set_focus_form() {
    $('#txt_nome').focus();
}

function set_dados_grid(dados) {
    return '<td>' + dados.Referencia + '</td>' +
        '<td>' + dados.TipoNegocio + '</td>' +
        '<td>' + dados.Bairro + '</td>' +
        '<td>' + dados.Cidade + '</td>' +
        '<td>' + dados.TipoImovel + '</td>' +
        '<td>' + dados.Preco + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Regiao: '',
        Referencia: '',
        TipoNegocio: '',
        Tipoimovel: '',
        Preco: 0,
        Dormitorio: 0,
        Vagas: 0,
        Area: 0,
        Cidade: '',
        Bairro: '',
        Ativo: true,
        Permuta: false,
        Descricao: '',
        Imagem: '',
    };
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Regiao: $('#ddl_regiao').val(),
        Referencia: $('#txt_referencia').val(),
        TipoNegocio: $('#ddl_negocio').val(),
        Tipoimovel: $('#ddl_tipoimovel :selected').text(),
        Preco: $('#txt_preco').val(),
        Dormitorio: $('#txt_dormitorio').val(),
        Vagas: $('#txt_vagas').val(),
        Area: $('#txt_area').val(),
        Cidade: $('#ddl_cidade :selected').text(),
        Bairro: $('#ddl_bairro :selected').text(),
        Ativo: $('#cbx_ativo').prop('checked'),
        Permuta: $('#cbx_permuta').prop('checked'),
        Descricao: $('#txt_descricao').val()
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
}


$(document).on('change', '#ddl_pais', function () {
    var ddl_pais = $(this),
        id_pais = parseInt(ddl_pais.val()),
        ddl_estado = $('#ddl_estado');

    if (id_pais > 0) {
        var url = url_listar_estados,
            param = { IdPais: id_pais };

        ddl_estado.empty();
        ddl_estado.prop('disabled', true);

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    ddl_estado.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                ddl_estado.prop('disabled', false);
            }
        });
    }

});

$("#ddl_estado").change(function () {
    var ddl_estado = $(this)
    id_estado = parseInt(ddl_estado.val()),
        ddl_cidade = $('#ddl_cidade');

    if (id_estado > 0) {
        var url = url_listar_cidades,
            param = { IdEstado: id_estado };

        ddl_cidade.empty();
        ddl_cidade.prop('disabled', true);

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    ddl_cidade.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                ddl_cidade.prop('disabled', false);
            }
        });
    }
});

$("#ddl_cidade").change(function () {
    var ddl_cidade = $(this)
    id_cidade = parseInt(ddl_cidade.val()),
        ddl_regiao = $('#ddl_regiao');

    if (id_cidade > 0) {

        ddl_regiao.prop('disabled', false);
    }
});


$("#ddl_regiao").change(function () {

    var ddl_regiao = $(this)
    nomeregiao = ddl_regiao.val(),
        id_cidade = parseInt(ddl_cidade.val()),
        ddl_bairro = $('#ddl_bairro');

    if (ddl_regiao != null) {

        var url = url_listar_bairro,
            param = { bairro: nomeregiao, Idcidade: id_cidade };

        ddl_bairro.empty();
        ddl_bairro.prop('disabled', true);

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    ddl_bairro.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                ddl_bairro.prop('disabled', false);
            }
        });
    }
});



$(document).on('click', '.number-spinner button', function () {
    var btn = $(this),
        oldValue = btn.closest('.number-spinner').find('input').val().trim(),
        newVal = 0;

    if (btn.attr('data-dir') == 'up') {
        newVal = parseInt(oldValue) + 1;
    } else {
        if (oldValue > 1) {
            newVal = parseInt(oldValue) - 1;
        } else {
            newVal = 0;
        }
    }
    btn.closest('.number-spinner').find('input').val(newVal);
});



$("#files").change(function () {
    var count = 0

    $($(this)[0].files).each(function () {

        count = count + 1;

    });

    var preview = $("#images .thumbnail").length;

    if (count > 12 || preview >= 12) {
        bootbox.alert({
            message: "no maximo 12 imagem",
            callback: function () {
                if (count > 5) {
                    document.getElementById("files").innerHTML.reload
                }
            }
        })

    }
    else {
        if (typeof (FileReader) != "undefined") {
            var dvpreview = $("#images");
            $($(this)[0].files).each(function () {
                var file = $(this);
                var reader = new FileReader();
                reader.onload = function (e) {

                    var number = Math.random() // 0.9394456857981651
                    number.toString(36); // '0.xtis06h6'
                    var id = number.toString(36).substr(2, 9); // 'xtis06h6'
                    id.length >= 9; // false

                    var html = '<div class="col-xs-3 col-md-3">';
                    html += '<div class="thumbnail" id="id">';
                    html += '<img width="100" height="100"  src=' + e.target.result + '>';
                    html += '<div class="caption">';
                    html += ' <p><a class="btn btn-danger btn-excluirimg" role ="button"><i class="glyphicon glyphicon-trash"></i></a></p>'
                    html += '</div>';
                    html += '</div>';
                    html += '</div>';


                    dvpreview.append(html);
                }
                reader.readAsDataURL(file[0]);

            });

        } else {

            alert("error");
        }
    }

});

$(document).on('click', '#btnUpload', function () {
    if (window.FormData !== undefined) {
        var fileUpload = $("#files").get(0);
        var files = fileUpload.files;
        var fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);

        }

        fileData.append('username', 'Faisal');
        $.ajax({
            url: incluir_imagens,
            type: "POST",
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                alert(result);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    } else {
        alert("FormData is not supported in the browser.");
    }
});



var salvar_customizado = null;


function marcar_ordenacao_campo(coluna) {
    var ordem_crescente = true,
        ordem = coluna.find('i');

    if (ordem.length > 0) {
        ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
        if (ordem_crescente) {
            ordem.removeClass('glyphicon-arrow-down');
            ordem.addClass('glyphicon glyphicon-arrow-up');
        }
        else {
            ordem.removeClass('glyphicon-arrow-up');
            ordem.addClass('glyphicon-arrow-down');
        }
    }
    else {
        $('.coluna-ordenacao i').remove();
        coluna.append('&nbsp;<i class="glyphicon glyphicon-arrow-down" style="color: #000000"></i>');
    }
}

function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function obter_ordem_grid() {
    var colunas_grid = $('.coluna-ordenacao'),
        ret = '';

    colunas_grid.each(function (index, item) {
        var coluna = $(item),
            ordem = coluna.find('i');

        if (ordem.length > 0) {
            ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
            ret = coluna.attr('data-campo') + (ordem_crescente ? '' : ' desc');
            return true;
        }
    });

    return ret;
}

function formatar_mensagem_aviso(mensagens) {
    var ret = '';

    for (var i = 0; i < mensagens.length; i++) {
        ret += '<li>' + mensagens[i] + '</li>';
    }

    return '<ul>' + ret + '</ul>';
}

function formatar_mensagem_erro(mensagens) {
    var ret = '';

    for (var i = 0; i < mensagens.length; i++) {
        ret += '<li>' + mensagens[i] + '</li>';
    }

    return '<ul>' + ret + '</ul>';
}

function abrir_form(dados) {
    set_dados_form(dados);

    var modal_cadastro = $('#modal_cadastro');
   
    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();

    bootbox.dialog({
        title: 'Cadastro de ' + tituloPagina,
        message: modal_cadastro,
        size: 'large'
    })
        .on('shown.bs.modal', function () {
            modal_cadastro.show(0, function () {
               
                set_focus_form();
            });
        })
        .on('hidden.bs.modal', function () {
            modal_cadastro.hide().appendTo('body');
        });
}

function salvar_ok(response, param) {
    if (response.Resultado == "OK") {
        if (param.Id == 0) {
            param.Id = response.IdSalvo;
            var table = $('#grid_cadastro').find('tbody'),
                linha = criar_linha_grid(param);

            table.append(linha);
            $('#grid_cadastro').removeClass('invisivel');
            $('#mensagem_grid').addClass('invisivel');
            $('#quantidade_registros').text(response.Quantidade);
        }
        else {
            var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
            preencher_linha_grid(param, linha);
        }

        $('#modal_cadastro').parents('.bootbox').modal('hide');

        var teste = $('#tab2default').html();
        bootbox.dialog({
            title: 'Imagens',
            message: teste,
            size: 'large'
        })
            .on('shown.bs.modal', function () {
                teste.show();
            })
            .on('hidden.bs.modal', function () {
                teste.hide().appendTo('body');
            });

    }
    else if (response.Resultado == "ERRO") {
        $('#msg_aviso').hide();
        $('#msg_mensagem_aviso').hide();
        $('#msg_erro').show();
    }
    else if (response.Resultado == "AVISO") {
        $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
        $('#msg_aviso').show();
        $('#msg_mensagem_aviso').show();
        $('#msg_erro').hide();
    }
}

function salvar_erro() {
    swal('Aviso', 'Não foi possível salvar. Tente novamente em instantes.', 'warning');
}

function criar_linha_grid(dados) {
    var ret =
        '<tr data-id=' + dados.Id + '>' +
        set_dados_grid(dados) +
        '<td>' +
        '<a class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Alterar</a>' +
        '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>' +
        '</td>' +
        '</tr>';

    return ret;
}

$(document).on('click', '#btn_incluir', function () {
    document.getElementById("imagenss").style.display = "none"
    document.getElementById("tab2default").style.display = "none"
    abrir_form(get_dados_inclusao());
})
    .on('click', '.btn-alterar', function () {
        var btn = $(this),
            id = btn.closest('tr').attr('data-id'),
            url = url_alterar,
            param = { 'id': id };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                abrir_form(response);
            }
        });

    })
    .on('click', '.btn-excluir', function () {
        var btn = $(this),
            tr = btn.closest('tr'),
            id = tr.attr('data-id'),
            url = url_excluir,
            param = { 'id': id };

        bootbox.confirm({
            message: "Realmente deseja excluir o " + tituloPagina + "?",
            buttons: {
                confirm: {
                    label: 'Sim',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'Não',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post(url, add_anti_forgery_token(param), function (response) {
                        if (response) {
                            tr.remove();
                            var quant = $('#grid_cadastro > tbody > tr').length;
                            if (quant == 0) {
                                $('#grid_cadastro').addClass('invisivel');
                                $('#mensagem_grid').removeClass('invisivel');
                            }
                        }
                    });
                }
            }
        });
    })
    .on('click', '#btn_confirmar', function () {

        var btn = $(this),
            url = url_confirmar,
            param = get_dados_form();

        if (salvar_customizado && typeof (salvar_customizado) == 'function') {
            salvar_customizado(url, param, salvar_ok, salvar_erro);
        }
        else {
            $.post(url, add_anti_forgery_token(param), function (response) {
                salvar_ok(response, param);
            })
            error: (function () {
                salvar_erro();
            });
        }
    })
    .on('click', '.page-item', function () {
        var btn = $(this),
            tamPag = $('#ddl_tam_pag').val(),
            pagina = btn.text(),
            url = url_page_click,
            param = { 'pagina': pagina, 'tamPag': tamPag };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }

                btn.siblings().removeClass('active');
                btn.addClass('active');
            }
        });
    })
    .on('change', '#ddl_tam_pag', function () {
        var ddl = $(this),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_tam_pag_change,
            param = { 'pagina': pagina, 'tamPag': tamPag };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        });
    })
    .on('keyup', '#txt_filtro', function () {
        var ordem = obter_ordem_grid(),
            filtro = $(this),
            ddl = $('#ddl_tam_pag'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_filtro_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    })
    .on('click', '.coluna-ordenacao', function () {
        marcar_ordenacao_campo($(this));

        var ordem = obter_ordem_grid(),
            filtro = $('#txt_filtro'),
            ddl = $('#ddl_tam_pag'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_filtro_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    });


$('#btn_sair').on('click', function () {

    location.reload();
});



