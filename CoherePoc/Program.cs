using Newtonsoft.Json;
using System.Text;

string apiKey = "API_KEY";

var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

var prompt = @"
Você é um engenheiro civil especializado em obras públicas.

Sua tarefa é analisar a MENSAGEM a seguir, considerando-a no contexto de um projeto de construção civil. Com base nas informações fornecidas, elabore uma lista no formato JSON, onde cada item terá os seguintes campos: nome_material, unidade_medida e quantidade_estimada.

Regras:

1. Se a MENSAGEM não fornecer dados suficientes para estimar qualquer item, retorne apenas: ""Dados insuficientes. Exemplo de mensagem adequada: 'Execução de fundação com sapatas isoladas em terreno argiloso para um prédio de 2 andares com 6 apartamentos, totalizando 300 m² de área construída.'""

2. Se o conteúdo não for relacionado à construção civil, retorne apenas: ""Mensagem inválida: conteúdo não relacionado à construção civil.""

3. Em casos válidos, não inclua nenhuma explicação — retorne somente o JSON.

4. Sempre tente estimar ao máximo possível com o que for informado. Só retorne erro se for realmente impossível inferir qualquer material.

5. Nunca desvie do tema construção civil.

6. Abrevie as unidades de medida, ex: UN., M², PÇ.

MENSAGEM:
{{MENSAGEM}}
";

//var mensagem = @"Quero construir uma casa térrea de 80m², com alvenaria estrutural, laje pré-fabricada, telhado de telha cerâmica, pisos cerâmicos, paredes internas com massa corrida e pintura látex, e fundação em sapata corrida. O terreno tem 200m² e precisa de aterramento com 30cm de altura.";
var mensagem = @"Quero construir uma casa simples de 80m², com dois quartos, sala, cozinha, banheiro, área de serviço e garagem. O terreno tem 200m² e precisa de aterramento";

// Prepare o payload
var requestBody = new
{
    model = "command-a-03-2025", // ou qualquer outro modelo de chat disponível
    messages = new[]
    {
        new { role = "user", content = prompt.Replace("{{MENSAGEM}}", mensagem) }
    }
};

var json = JsonConvert.SerializeObject(requestBody);
var content = new StringContent(json, Encoding.UTF8, "application/json");

// Enviar o pedido à API
var response = await client.PostAsync("https://api.cohere.ai/v2/chat", content);
if (response.IsSuccessStatusCode)
{
    var responseString = await response.Content.ReadAsStringAsync();
    var responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);

    // Exibir a resposta do modelo
    Console.WriteLine($"Resposta: {responseJson?.message?.content[0]?.text}");
}
else
{
    Console.WriteLine("Erro ao acessar a API: " + response.StatusCode);
}