$(document).ready(function () {

    var clienteId = 0; // Variável para armazenar o Id do Cliente

    var nome = $('#NomeBeneficiario').val();
    var cpf = $('#CPFBeneficiario').val();

    $('#modalTitle').text('Adicionar Dependente');
    $('#beneficiarioId').val('');

    // Listar beneficiarios
    $('#beneficiariosModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Botão que acionou o modal
        clienteId = button.data('cliente-id'); // Extrai o id do cliente do data attribute

        IncluirButton();

        var urlBeneficiarioList = '/Cliente/BeneficiarioList?clienteId=' + clienteId;

        if (document.getElementById("gridBeneficiarios"))
            $('#gridBeneficiarios').jtable({
                title: 'Beneficiarios',
                //paging: true, //Enable paging
                //pageSize: 5, //Set page size (default: 10)
                sorting: true, //Enable sorting
                defaultSorting: 'Nome ASC', //Set default sorting
                actions: {
                    listAction: urlBeneficiarioList,
                },
                fields: {
                    CPF: {
                        title: 'CPF',
                        width: '30%',
                        class: 'CPF'
                    },
                    Nome: {
                        title: 'Nome',
                        width: '40%'
                    },
                    Açoes: {
                        title: '',
                        width: '30%',
                        sorting: false,
                        display: function (data) {
                            return `<button class="btn btn-primary btn-sm editar-beneficiario" data-id="${data.record.Id}">Alterar</button>` +
                                `<button class="btn btn-danger btn-sm excluir-beneficiario" data-id="${data.record.Id}">Excluir</button>`;
                        }
                    }
                }
            });

        if (document.getElementById("gridBeneficiarios"))
            $('#gridBeneficiarios').jtable('load');        
    });

    // Incluir novo Beneficiario
    $('#salvarBeneficiario').click(function () {
                
        var acao = $('#salvarBeneficiario').text();

        var nome = $('#NomeBeneficiario').val();
        var cpf = $('#CPFBeneficiario').val();

        if (nome && cpf) {
            if (acao === "Incluir") {
                incluirBeneficiario(cpf, nome);
            } else if (acao === "Alterar") {
                editarBeneficiario(cpf, nome);
            }
        } else {
            ModalDialog("Atenção", "Preencha todos os campos.");
        }
    });

    function AlterarButton() {
        $('#salvarBeneficiario').text('Alterar');
        $('#salvarBeneficiario').removeClass('btn-success');
        $('#salvarBeneficiario').addClass('btn-warning');
    }
    function IncluirButton() {
        $('#salvarBeneficiario').text('Incluir');
        $('#salvarBeneficiario').removeClass('btn-warning');
        $('#salvarBeneficiario').addClass('btn-success');
    }
    function incluirBeneficiario(cpf, nome) {
        $.ajax({
            url: '/Cliente/IncluirBeneficiario', // URL da sua Action no Controller
            type: 'POST',
            data: { 
                idCliente: clienteId,
                cpf: cpf,
                nome: nome
            },
            success: function (response) {
                if (response.success) {
                    // Atualiza a lista de beneficiarios
                    $('#gridBeneficiarios').jtable('load', {
                        clienteId: $('#dependentesModal').data('cliente-id')  // Id do Cliente para filtrar dependentes
                    });                    

                    ModalDialog("Aviso", "Beneficiario incluído com sucesso");
                } else {
                    ModalDialog("Erro ao salvar beneficiario!!!: ", response.message);
                }
            },
            error: function (r) {
                if (r.status == 400)
                    ModalDialog("Erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Erro", "Erro ao salvar beneficiario@@.");
            }
        });
        $('#nomeBeneficiario').val('');
        $('#cpfBeneficiario').val('');
    }
    function editarBeneficiario(cpf, nome) {
        var idBeneficiario = $('#beneficiarioId').val();
        $.ajax({
            url: '/Cliente/AlterarBeneficiario', // URL da sua Action no Controller
            type: 'POST',
            data: {
                id: idBeneficiario,
                idCliente: clienteId,
                cpf: cpf,
                nome: nome
            },
            success: function (response) {
                if (response.success) {
                    // Atualiza a lista de beneficiarios
                    $('#gridBeneficiarios').jtable('load', {
                        clienteId: $('#dependentesModal').data('cliente-id')  // Id do Cliente para filtrar dependentes
                    });

                    ModalDialog("Aviso", "Beneficiario atualizado com sucesso");
                    IncluirButton();
                    $('#beneficiarioId').val('');
                    $('#NomeBeneficiario').val('');
                    $('#CPFBeneficiario').val('');

                } else {
                    ModalDialog("Erro ao salvar beneficiario: ", response.message);
                }
            },
            error: function (r) {
                if (r.status == 400)
                    ModalDialog("Erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Erro", "Erro ao salvar beneficiario@@.");
            }
        });
    }

    // Excluir beneficiario
    $(document).on('click', '.excluir-beneficiario',
        function () {
            var Id = $(this).data('id'); // Captura o id do beneficiario

            if (confirm('Você tem certeza que deseja excluir este beneficiario?')) {
                $.ajax({
                    url: '/Cliente/ExcluiBeneficiario',
                    type: 'POST',
                    data: { id: Id },  // Envia o id do beneficiario para exclusão
                    success: function (response) {
                        if (response.success) {
                            // Remove a linha da tabela (atualiza a tabela após exclusão)
                            $('#gridBeneficiarios').jtable('load', {
                                clienteId: $('#dependentesModal').data('cliente-id')  // Id do Cliente para filtrar dependentes
                            });
                        } else {
                            ModalDialog("Erro ao excluir beneficiario: ", response.message);
                        }
                    },
                    error: function () {
                        ModalDialog("Erro", "Erro ao excluir beneficiario.");
                    }
                });
            }
            if (document.getElementById("gridBeneficiarios"))
                $('#gridBeneficiarios').jtable('load');  
        });

    //Editar beneficiario
    $(document).on('click', '.editar-beneficiario', function () {
        var Id = $(this).data('id'); // Captura o id do beneficiario

        // Faz uma requisição AJAX para buscar os dados do beneficiario
        $.ajax({
            url: '/Cliente/ConsultarBeneficiario', // Rota para buscar dados do beneficiario
            type: 'GET',
            data: { idBeneficiario: Id },
            success: function (response) {
                if (response.success) {
                    // Preenche o modal com os dados do beneficiario                    
                    $('#beneficiarioId').val(response.data.Id);
                    $('#NomeBeneficiario').val(response.data.Nome);
                    $('#CPFBeneficiario').val(response.data.CPF);

                    // Ajusta o título do modal para edição
                    AlterarButton();

                    // Abre o modal
                    /*$('#beneficiariosModal').modal('show');*/
                } else {
                    alert('Erro ao buscar dados do beneficiario: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.log("Erro no AJAX: " + error);
                alert('Erro ao buscar os dados do dependente.');
            }
        });
    });
})