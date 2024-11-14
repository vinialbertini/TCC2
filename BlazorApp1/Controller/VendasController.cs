using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using BlazorApp1.Pages;
using NuvemFiscal.Sdk.Api;
using NuvemFiscal.Sdk.Client;
using NuvemFiscal.Sdk.Model;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Differencing;
using Polly.Caching;
using Azure;

namespace BlazorApp1.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venda>>> GetVenda()
        {
            try
            {
                // Retorna todas as vendas do banco de dados, incluindo os itens associados
                var vendas = await _context.Vendas
                    .Include(v => v.Cliente)
                    .Include(v => v.Itens) // Inclui os itens do pedido
                    .ToListAsync();

                if (vendas == null)
                {
                    return NotFound();
                }

                return vendas;
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um código de status 500 Internal Server Error
                // Juntamente com uma mensagem de erro
                return StatusCode(500, $"Erro ao buscar as vendas: {ex.Message}");
            }
        }

        // GET: api/Vendas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venda>> GetVenda(int id)
        {
            try
            {
                // Recupera o pedido pelo ID, incluindo os itens relacionados
                var venda = await _context.Vendas
                    .Include(v => v.Itens) // Inclui os itens do pedido
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound();
                }

                return venda;
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um código de status 500 Internal Server Error
                // Juntamente com uma mensagem de erro
                return StatusCode(500, $"Erro ao buscar o pedido: {ex.Message}");
            }
        }

        // PUT: api/Vendas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenda(int id, Venda venda)
        {
            if (id != venda.Id)
            {
                return BadRequest();
            }

            try
            {
                // Carrega a venda do banco de dados, incluindo os itens associados
                var vendaExistente = await _context.Vendas
                    .Include(v => v.Itens)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vendaExistente == null)
                {
                    return NotFound();
                }

                // Atualiza as propriedades do pedido
                _context.Entry(vendaExistente).CurrentValues.SetValues(venda);

                // Remove os itens do pedido que não estão mais presentes no objeto 'venda'
                foreach (var itemExistente in vendaExistente.Itens.ToList())
                {
                    if (!venda.Itens.Any(i => i.Id == itemExistente.Id))
                    {
                        _context.Itens.Remove(itemExistente);
                    }
                }

                // Adiciona ou atualiza os itens do pedido
                foreach (var item in venda.Itens)
                {
                    var itemExistente = vendaExistente.Itens.FirstOrDefault(i => i.Id == item.Id);
                    if (itemExistente != null)
                    {
                        _context.Entry(itemExistente).CurrentValues.SetValues(item);
                    }
                    else
                    {
                        vendaExistente.Itens.Add(item);
                    }
                }

                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um código de status 500 Internal Server Error
                // Juntamente com uma mensagem de erro
                return StatusCode(500, $"Erro ao atualizar o pedido: {ex.Message}");
            }
        }

        // POST: api/Vendas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Venda>> PostVenda(Venda venda)
        {
            //if (_context.Vendas == null)
            //{
            //    return Problem("Entity set 'AppDbContext.Venda'  is null.");
            //}
            if (venda == null || venda.Itens == null || !venda.Itens.Any())
            {
                return BadRequest("Pedido inválido. Deve conter pelo menos um item.");
            }
            venda.Id_Empresa = 1;
            //venda.Id_Cliente = 1;
            
            _context.Vendas.Add(venda);

            foreach (var item in venda.Itens)
            {
                // Defina o ID do pedido para o item
                item.Id_Venda = venda.Id;

                // Adiciona o item ao contexto do EF
                _context.Itens.Add(item);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVenda", new { id = venda.Id }, venda);
        }

        // DELETE: api/Vendas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(int id)
        {
            try
            {
                var venda = await _context.Vendas
                    .Include(v => v.Itens) // Inclui os itens do pedido
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound();
                }

                // Remove os itens do pedido
                _context.Itens.RemoveRange(venda.Itens);

                // Remove o pedido
                _context.Vendas.Remove(venda);

                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna um código de status 500 Internal Server Error
                // Juntamente com uma mensagem de erro
                return StatusCode(500, $"Erro ao excluir o pedido: {ex.Message}");
            }
        }

        private bool VendaExists(int id)
        {
            return (_context.Vendas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        // GET: api/Vendas/TopClientes
        [HttpGet("TopClientes")]
        public async Task<ActionResult<IEnumerable<TopClienteDTO>>> GetTopClientes()
        {
            try
            {
                var topClientes = await _context.Vendas
                    .Include(v => v.Cliente) // Inclui as informações do cliente
                    .GroupBy(v => v.Cliente) // Agrupa as vendas por cliente
                    .Select(g => new TopClienteDTO // Altera aqui para usar a nova classe DTO
                    {
                        Id_Cliente = g.Key.Id,
                        //ClienteNome = g.Key.Nome.Trim(),
                        ClienteNome = g.Key.Id + "-" + g.Key.Nome,
                        TotalComprado = g.Sum(v => v.Total) // Calcula o total gasto pelo cliente
                    })
                    .OrderByDescending(c => c.TotalComprado) // Ordena em ordem decrescente
                    .Take(5) // Pega os 5 primeiros
                    .ToListAsync();

                return Ok(topClientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar os principais clientes: {ex.Message}");
            }
        }
        // GET: api/Vendas/TopClientesMaisCompras
        [HttpGet("TopClientesMaisCompras")]
        public async Task<ActionResult<IEnumerable<TopClienteDTO>>> GetTopClientesMaisCompras()
        {
            try
            {
                var topClientesMaisCompras = await _context.Vendas
                    .Include(v => v.Cliente) // Inclui as informações do cliente
                    .GroupBy(v => v.Cliente) // Agrupa as vendas por cliente
                    .Select(g => new TopClienteDTO
                    {
                        Id_Cliente = g.Key.Id,
                        ClienteNome = g.Key.Id + "-" + g.Key.Nome,
                        TotalComprado = g.Count() // Conta o número de transações para cada cliente
                    })
                    .OrderByDescending(c => c.TotalComprado) // Ordena em ordem decrescente
                    .Take(5) // Pega os 5 primeiros
                    .ToListAsync();

                return Ok(topClientesMaisCompras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar os clientes que mais compraram: {ex.Message}");
            }
        }
        // GET: api/Vendas/TopProdutos
        [HttpGet("TopProdutos")]
        public async Task<ActionResult<IEnumerable<TopProdutoDTO>>> GetTopProdutos()
        {
            try
            {
                var topProdutos = await _context.Itens
                    .Include(i => i.Produto) // Inclui as informações do produto
                    .GroupBy(i => i.Produto) // Agrupa os itens por produto
                    .Select(g => new TopProdutoDTO
                    {
                        ProdutoId = g.Key.Id,
                        ProdutoNome = g.Key.Descricao,
                        QuantidadeVendida = g.Sum(i => i.Quantidade) ?? 0 // Soma a quantidade vendida de cada produto
                    })
                    .OrderByDescending(p => p.QuantidadeVendida) // Ordena em ordem decrescente pela quantidade
                    .Take(5) // Pega os 5 primeiros produtos mais vendidos
                    .ToListAsync();

                return Ok(topProdutos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar os produtos mais vendidos: {ex.Message}");
            }
        }
 

        public string GerarGtin(int produtoId)
        {
            // Converte o Id para string e completa com zeros à esquerda para formar 12 dígitos
            string baseGtin = produtoId.ToString().PadLeft(12, '0');

            // Calcula o dígito verificador
            int checkDigit = CalcularDigitoVerificadorGtin(baseGtin);

            // Retorna o GTIN completo de 13 dígitos
            return baseGtin + checkDigit;
        }

        private int CalcularDigitoVerificadorGtin(string baseGtin)
        {
            int sum = 0;

            // Multiplica cada dígito por 3 ou 1, alternando a cada posição
            for (int i = 0; i < baseGtin.Length; i++)
            {
                int digit = int.Parse(baseGtin[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            // Calcula o dígito verificador
            int mod = sum % 10;
            int checkDigit = (mod == 0) ? 0 : 10 - mod;

            return checkDigit;
        }
        [HttpPost("{id}/EmitirNotaFiscal")]
        public async Task<ActionResult> EmitirNotaFiscal(int id)
        {
            try
            {
                // Obter a venda pelo ID e incluir os dados necessários
                var venda = await _context.Vendas
                    .Include(v => v.Empresas)
                    .Include(v => v.Cliente)
                    .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null)
                {
                    return NotFound("Venda não encontrada.");
                }

                // Configurar a NuvemFiscal
                Configuration config = new Configuration();
                config.BasePath = "https://api.nuvemfiscal.com.br";
                config.AccessToken = "YOUR_ACCESS_TOKEN";

                HttpClient httpClient = new HttpClient();
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                var apiInstance = new NfceApi(httpClient, config, httpClientHandler);

                // Preparar o pedido de emissão de NFC-e
                var pedidoEmissao = new NfePedidoEmissao
                {
                    ambiente = NfePedidoEmissao.AmbienteEnum.Homologacao, // para homologação
                                                                          // ambiente = NfePedidoEmissao.AmbienteEnum.Producao // para produção

                    // Preencher com os dados específicos da venda

                    infNFe = new NfeSefazInfNFe
                    {
                        versao = "4.00",  // Versão da NF-e
                        Id = Guid.NewGuid().ToString(),  // ID da NF-e, normalmente gerado automaticamente
                        ide = new NfeSefazIde
                        {
                            cUF = 41,  // Código da UF
                            cNF = Guid.NewGuid().ToString("N"),  // Código de segurança da NF-e
                            natOp = "Venda",  // Natureza da operação
                            mod = 55,  // Modelo da NF-e
                            serie = venda.Serie,  // Série da NF-e
                            nNF = venda.Nr_Nota,  // Número da NF-e
                            dhEmi = DateTime.UtcNow,  // Data de emissão
                            tpNF = 2,  // Tipo de NF-e (1 = entrada, 2 = saída)
                            tpAmb = 2,  // Tipo de ambiente (1 = produção, 2 = homologação)
                                        // Outros campos de Ide podem ser preenchidos conforme a necessidade
                        },
                        emit = new NfeSefazEmit
                        {
                            CNPJ = venda.Empresas.CNPJ,
                            xNome = venda.Empresas.Razao_Social,
                            xFant = venda.Empresas.Fantasia,
                            enderEmit = new NfeSefazEnderEmi
                            {
                                xLgr = venda.Empresas.Endereco,
                                nro = venda.Empresas.Numero.ToString(),
                                xCpl = venda.Empresas.Complemento,
                                xBairro = venda.Empresas.Bairro,
                                cMun = "4106902",//venda.Empresas.
                                xMun = venda.Empresas.Cidade,
                                UF = venda.Empresas.UF,
                                CEP = venda.Empresas.CEP
                            },
                            //IE = venda.Empresas.IE,
                            // Outros campos do emitente
                        },
                        dest = new NfeSefazDest
                        {
                            CNPJ = venda.Cliente.CPF_CNPJ.Length == 14 ? venda.Cliente.CPF_CNPJ : null,
                            CPF = venda.Cliente.CPF_CNPJ.Length == 11 ? venda.Cliente.CPF_CNPJ : null,
                            xNome = venda.Cliente.Nome,
                            enderDest = new NfeSefazEndereco
                            {
                                xLgr = venda.Cliente.Endereco,
                                nro = venda.Cliente.Numero.ToString(),
                                xCpl = venda.Cliente.Complemento,
                                xBairro = venda.Cliente.Bairro,
                                cMun = "4106902",//venda.Empresas.
                                xMun = venda.Cliente.Cidade,
                                UF = venda.Cliente.Estado,
                                CEP = venda.Cliente.CEP
                            },

                            // Outros campos do destinatário
                        },
                        det = venda.Itens.Select((item, index) => new NfeSefazDet
                        {
                            nItem = index + 1,
                            prod = new NfeSefazProd
                            {
                                cProd = item.Produto.Id.ToString(),
                                cEAN = GerarGtin(item.Produto.Id),
                                xProd = item.Produto.Descricao,
                                NCM = "85367000", //item.Produto.NCM,
                                CFOP = "5102",//item.Produto.CFOP,
                                uCom = item.Produto.UN,
                                qCom = item.Quantidade,
                                vUnCom = item.Valor,
                                vProd = item.Total,
                                cEANTrib = GerarGtin(item.Produto.Id),
                                uTrib = item.Produto.UN,
                                qTrib = item.Quantidade,
                                vUnTrib = item.Valor,
                                indTot = 1,
                                // Outros campos do produto
                            },
                            imposto = new NfeSefazImposto
                            {
                                ICMS = new NfeSefazICMS
                                {
                                    ICMS00 = new NfeSefazICMS00
                                    {
                                        orig = 0,  // Origem da mercadoria
                                        CST = "00",  // Código de Situação Tributária
                                        modBC = 0,  // Modo de base de cálculo
                                        vBC = item.Total,  // Valor da base de cálculo
                                        pICMS = 18,  // Alíquota do ICMS
                                        vICMS = item.Total * 0.18m  // Valor do ICMS
                                    }
                                }
                            }

                            // Outros campos podem ser preenchidos aqui

                        }).ToList(),
                        total = new NfeSefazTotal
                        {
                            ICMSTot = new NfeSefazICMSTot
                            {
                                vBC = venda.Total,
                                vICMS = 0,
                                vFCP = 0,
                                vBCST = 0,
                                vST = 0,
                                vFCPST = 0,
                                vFCPSTRet = 0,
                                vProd = venda.Total,
                                vFrete = 0,
                                vSeg = 0,
                                vDesc = 0,
                                vII = 0,
                                vIPI = 0,
                                vIPIDevol = 0,
                                vPIS = 0,
                                vCOFINS = 0,
                                vOutro = 0,
                                vNF = venda.Total
                            }
                        },
                        transp = new NfeSefazTransp
                        {
                            modFrete = 1
                        },
                        pag = new NfeSefazPag
                        {
                            detPag = new List<NfeSefazDetPag>
                           {
                               new NfeSefazDetPag
                               {
                                   tPag = "01",
                                   vPag = venda.Total
                               }
                           }
                        }
                    }//infNFe = new NfeSefazInfNFe
                };//var pedidoEmissao = new NfePedidoEmissao

                // Emitir NFC-e através da API da NuvemFiscal
                Dfe result = apiInstance.EmitirNfce(pedidoEmissao);
                Debug.WriteLine(result);
                return Ok(result);

            } //try
            catch (ApiException e)
            {
                // Tratamento de erro específico para a API da NuvemFiscal
                Debug.Print("Exception when calling NfceApi.EmitirNfce: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
                return StatusCode(500, "Erro ao emitir a NFC-e: " + e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno ao processar a requisição: " + ex.Message);
            }
        }//EmitirNotaFiscal
    } //VendasController : ControllerBase
}//BlazorApp1.Controller
