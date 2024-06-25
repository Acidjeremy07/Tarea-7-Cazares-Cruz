const apiBaseUrl = 'https://t7-2021630179-a.azurewebsites.net/api';

async function createArticle(article) {
    const response = await fetch(`${apiBaseUrl}/createarticle`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(article)
    });
    return response.json();
}

async function searchArticles(keyword) {
    const response = await fetch(`${apiBaseUrl}/searcharticles`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ keyword })
    });
    return response.json();
}

async function buyArticle(id, quantity) {
    const response = await fetch(`${apiBaseUrl}/buyarticle`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id, quantity })
    });
    return response.json();
}

async function removeArticleFromCart(id) {
    const response = await fetch(`${apiBaseUrl}/removearticlefromcart`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ id })
    });
    return response.json();
}

async function clearCart() {
    const response = await fetch(`${apiBaseUrl}/clearcart`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    });
    return response.json();
}
