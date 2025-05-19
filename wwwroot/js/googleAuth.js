let auth2;

function initializeGoogleSignIn() {
    return new Promise((resolve, reject) => {
        if (typeof gapi === 'undefined') {
            reject('Google API not loaded');
            return;
        }

        gapi.load('auth2', () => {
            gapi.auth2.init({
                client_id: '760923995061-c82tbb03e5pmbrldu0hvoffl3lk7naou.apps.googleusercontent.com'
            }).then((authInstance) => {
                auth2 = authInstance;
                resolve();
            }).catch(reject);
        });
    });
}

function signInWithGoogle() {
    return new Promise((resolve, reject) => {
        if (!auth2) {
            reject('Google Auth not initialized');
            return;
        }

        auth2.signIn().then((googleUser) => {
            const idToken = googleUser.getAuthResponse().id_token;
            resolve(idToken);
        }).catch(reject);
    });
} 