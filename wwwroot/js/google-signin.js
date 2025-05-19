let googleUser = null;

function initializeGoogleSignIn() {
    gapi.load('auth2', function() {
        gapi.auth2.init({
            client_id: 'YOUR_GOOGLE_CLIENT_ID', // You'll need to replace this with your actual Google Client ID
            scope: 'profile email'
        }).then(function(auth2) {
            googleUser = auth2;
        });
    });
}

async function signInWithGoogle() {
    try {
        const auth2 = gapi.auth2.getAuthInstance();
        const googleUser = await auth2.signIn();
        const idToken = googleUser.getAuthResponse().id_token;
        return idToken;
    } catch (error) {
        console.error('Google Sign-In Error:', error);
        return null;
    }
} 