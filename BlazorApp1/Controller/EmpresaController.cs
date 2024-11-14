using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuvemFiscal.Sdk.Api;
using NuvemFiscal.Sdk.Client;
using NuvemFiscal.Sdk.Model;

namespace BlazorApp1.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpresaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Empresa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresas>>> GetEmpresas()
        {
            if (_context.Empresa == null)
            {
                return NotFound();
            }
            return await _context.Empresa.ToListAsync();
        }

        // GET: api/Empresa/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empresas>> GetEmpresa(int id)
        {
            if (_context.Empresa == null)
            {
                return NotFound();
            }
            var empresa = await _context.Empresa.FindAsync(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return empresa;
        }


        // PUT: api/Empresa/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpresa(int id, Empresas empresa)
        {
            if (id != empresa.Id)
            {
                return BadRequest();
            }

            _context.Entry(empresa).State = EntityState.Modified;

            try
            {
                // Atualiza a empresa no banco de dados
                await _context.SaveChangesAsync();

                // Configuração da API da Nuvem Fiscal
                Configuration config = new Configuration
                {
                    BasePath = "https://api.nuvemfiscal.com.br"
                };
                config.AddApiKey("Authorization", "YOUR_API_KEY");
                config.AccessToken = "YOUR_ACCESS_TOKEN";

                // Criação do objeto de API para a Nuvem Fiscal
                var apiInstance = new EmpresaApi(new HttpClient(), config, new HttpClientHandler());

                // Construindo o objeto Empresa da Nuvem Fiscal
                var cnpjEmpresa = empresa.CNPJ; // CPF ou CNPJ da empresa sem máscara
                var atualizarEmpresa = new Empresa
                {
                    cpf_cnpj = empresa.CNPJ,
                    nome_razao_social = empresa.Razao_Social,
                    nome_fantasia = empresa.Fantasia,
                    email = empresa.Email,
                    endereco = new EmpresaEndereco
                    {
                        logradouro = empresa.Endereco,
                        numero = empresa.Numero.ToString(),
                        complemento = empresa.Complemento,
                        bairro = empresa.Bairro,
                        codigo_municipio = "4106902", // Ajuste conforme necessário
                        cidade = empresa.Cidade,
                        uf = empresa.UF,
                        cep = empresa.CEP
                    }
                };

                // Chamada à API para atualizar a empresa na Nuvem Fiscal
                Empresa result = await apiInstance.AtualizarEmpresaAsync(cnpjEmpresa, atualizarEmpresa);
                Debug.WriteLine($"Empresa atualizada com sucesso na Nuvem Fiscal: {result}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Erro ao atualizar na Nuvem Fiscal
                return StatusCode(500, $"Erro ao atualizar a empresa na Nuvem Fiscal: {ex.Message}");
            }

            return NoContent();
        }


        // POST: api/Empresa        
        [HttpPost]
        public async Task<ActionResult<Empresas>> PostEmpresa(Empresas empresa)
        {
            if (_context.Empresa == null)
            {
                return Problem("Entity set 'AppDbContext.Empresa'  is null.");
            }
            // Configuração da API da Nuvem Fiscal
            Configuration config = new Configuration
            {
                BasePath = "https://api.nuvemfiscal.com.br"
            };
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            // Criação do objeto de API para a Nuvem Fiscal
            var apiInstance = new EmpresaApi(new HttpClient(), config, new HttpClientHandler());

            // Construindo o objeto Empresa da Nuvem Fiscal
            var novaEmpresa = new Empresa
            {
                cpf_cnpj = empresa.CNPJ,
                //inscricao_estadual = empresa.InscricaoEstadual,
                //inscricao_municipal = empresa.InscricaoMunicipal,
                nome_razao_social = empresa.Razao_Social,
                nome_fantasia = empresa.Fantasia,
                //fone = empresa.Fone,
                email = empresa.Email,
                endereco = new EmpresaEndereco
                {
                    logradouro = empresa.Endereco,
                    numero = empresa.Numero.ToString(),
                    complemento = empresa.Complemento,
                    bairro = empresa.Bairro,
                    codigo_municipio = "4106902",//empresa.CodigoMunicipio,
                    cidade = empresa.Cidade,
                    uf = empresa.UF,
                    //CodigoPais = empresa.Endereco.CodigoPais,
                    //Pais = empresa.Endereco.Pais,
                    cep = empresa.CEP
                }
            };
            try
            {
                // Chamada à API para cadastrar a empresa na Nuvem Fiscal
                Empresa result = await apiInstance.CriarEmpresaAsync(novaEmpresa);
                Debug.WriteLine($"Empresa cadastrada com sucesso na Nuvem Fiscal: {result}");

                // Cadastro do certificado da empresa
                var certificadoBody = new EmpresaPedidoCadastroCertificado
                {
                    certificado = empresa.Certificado, // assumindo que está em Base64
                    password = empresa.Senha_Certificado
                };

                EmpresaCertificado resultCertificado = await apiInstance.CadastrarCertificadoEmpresaAsync(empresa.CNPJ, certificadoBody);
                Debug.WriteLine($"Certificado cadastrado com sucesso para empresa: {resultCertificado}");


                // Salvar no banco de dados local, se necessário
                _context.Empresa.Add(empresa);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetEmpresa", new { id = empresa.Id }, empresa);
            }
            catch (ApiException e)
            {
                Debug.Print("Erro ao chamar EmpresaApi.CriarEmpresa: " + e.Message);
                return StatusCode(e.ErrorCode, "Erro ao cadastrar empresa na Nuvem Fiscal.");
            }
        }


        // DELETE: api/Empresa/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            if (_context.Empresa == null)
            {
                return NotFound();
            }
            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresa.Remove(empresa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpresaExists(int id)
        {
            return (_context.Empresa?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
