const apiBaseUrl = 'https://your-azure-function-url';

async function createArticle(article) {
    const response = await fetch(`${apiBaseUrl}/CreateArticleFunction`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(article)
    });
    return response.json();
}

async function searchArticles(keyword) {
    const response = await fetch(`${apiBaseUrl}/SearchArticlesFunction`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ keyword })
    });
    return response.json();
}

async function buyArticle(id, quantity) {
    const response = await fetch(`${apiBaseUrl}/BuyArticleFunction`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id, quantity })
    });
    return response.json();
}

async function removeArticleFromCart(id) {
    const response = await fetch(`${apiBaseUrl}/RemoveArticleFromCartFunction`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id })
    });
    return response.json();
}

async function clearCart() {
    const response = await fetch(`${apiBaseUrl}/ClearCartFunction`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    return response.json();
}
