﻿function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_email').val(dados.Email);
    $('#txt_login').val(dados.Login);
    $('#txt_senha').val(dados.Senha);
    $('#cbx_ativo').prop('checked', dados.Ativo);
    $('#ddl_perfil').val(dados.IdPerfil);
  
}
function set_focus_form() {
    $('#txt_nome').focus()
}
function set_dados_grid(dados) {
    return '<td>' + dados.Nome + '</td>' +
        '<td>' + dados.Login + '</td>' +
        '<td>' + (dados.Ativo ? 'SiM' : 'NÃO') + '</td>';
}
function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Email: '',
        Login: '',
        Senha: '',
        Ativo: true,
        IdPerfil: 0
      
    };
}
function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Email: $('#txt_email').val(),
        Login: $('#txt_login').val(),
        Senha: $('#txt_senha').val(),
        Ativo: $('#cbx_ativo').prop('checked'),
        IdPerfil: $('#ddl_perfil').val()
     
    };
}
function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Login)
        .eq(2).html(param.Ativo ? 'SIM' : 'NÃO');
}




