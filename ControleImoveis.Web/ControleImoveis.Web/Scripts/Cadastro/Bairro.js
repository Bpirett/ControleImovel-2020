function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#ddl_pais').val(dados.IdPais);
    $('#cbx_ativo').prop('checked', dados.Ativo);

    $('#ddl_estado').val(dados.IdEstado);
    $('#ddl_estado').prop('disabled', dados.IdEstado <= 0 || dados.IdEstado == undefined);
    $('#ddl_cidade').val(dados.IdCidade);
    $('#ddl_cidade').prop('disabled', dados.IdCidade <= 0 || dados.IdCidade == undefined);
    $('#ddl_regiao').val(dados.NomeRegiao);
    $('#ddl_regiao').prop('disabled', dados.NomeRegiao <= 0 || dados.NomeRegiao == undefined);

}

function set_focus_form() {
    $('#txt_nome').focus();
}

function set_dados_grid(dados) {
    return '<td>' + dados.Nome + '</td>' +
        '<td>' + dados.IdCidade  + '</td>' +
        '<td>' + dados.NomeRegiao + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        IdPais: 0,
        IdEstado: 0,
        IdCidade: 0,
        Regiao: '',
        Ativo: true
    };
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        IdPais: $('#ddl_pais').val(),
        IdEstado: $('#ddl_estado').val(),
        IdCidade: $('#ddl_cidade').val(),
        NomeRegiao: $('#ddl_regiao').val(),
        Ativo: $('#cbx_ativo').prop('checked')
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

