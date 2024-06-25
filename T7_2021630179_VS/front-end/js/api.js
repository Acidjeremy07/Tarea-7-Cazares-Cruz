const apiBaseUrl = 'https://t7-2021630179-a.azurewebsites.net/api';

async function altaArticulo(article) {
    const response = await fetch(`${apiBaseUrl}/alta_articulo`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ articulo: article })
    });
    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }
    return response.json();
}

async function createArticle(article) {
    const response = await fetch(`${apiBaseUrl}/CreateArticle`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(article)
    });
    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }
    return response.json();
}

async function searchArticles(keyword) {
    const response = await fetch(`${apiBaseUrl}/SearchArticles`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ keyword })
    });
    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }
    return response.json();
}

async function buyArticle(id, quantity) {
    const response = await fetch(`${apiBaseUrl}/BuyArticle`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id, quantity })
    });
    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }
    return response.json();
}

async function removeArticleFromCart(id) {
    const response = await fetch(`${apiBaseUrl}/RemoveArticleFromCart`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id })
    });
    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }
    return response.json();
}
