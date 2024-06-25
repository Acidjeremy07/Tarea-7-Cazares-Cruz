document.getElementById('searchForm').addEventListener('submit', async (event) => {
    event.preventDefault();

    const keyword = document.getElementById('keyword').value;
    try {
        const results = await searchArticles(keyword);

        const resultsDiv = document.getElementById('results');
        resultsDiv.innerHTML = '';

        results.forEach(article => {
            const articleDiv = document.createElement('div');
            articleDiv.classList.add('article');

            articleDiv.innerHTML = `
                <img src="data:image/png;base64,${article.photo}" alt="${article.name}">
                <h2>${article.name}</h2>
                <p>${article.description}</p>
                <p>Precio: ${article.price}</p>
                <label for="quantity_${article.id}">Cantidad:</label>
                <input type="number" id="quantity_${article.id}" name="quantity" value="1">
                <button onclick="purchaseArticle(${article.id}, document.getElementById('quantity_${article.id}').value)">Comprar</button>
            `;

            resultsDiv.appendChild(articleDiv);
        });
    } catch (error) {
        console.error('Error:', error);
        alert('Error al buscar artículos');
    }
});

async function purchaseArticle(id, quantity) {
    try {
        const result = await buyArticle(id, quantity);
        alert(result.message);
    } catch (error) {
        console.error('Error:', error);
        alert('Error al comprar el artículo');
    }
}
