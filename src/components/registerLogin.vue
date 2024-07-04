<!-- registerLogin.vue -->
<template>
  <div class="bg-white w-3/5 m-auto rounded-xl shadow-xl mt-12">
    <header class="flex justify-between border-b border-slate-300 px-8">
      <div class="items-center gap-4">
        <router-link to="/">
          <img src="@/assets/logo1.png" alt="Logo" />
          <div>
            <h2 class="text-xl font-bold uppercase">ТГК-2 Энергосбыт</h2>
            <p class="text-slate-400 mb-4">Конвертации/Транзакции/Отчеты</p>
          </div>
        </router-link>
      </div>
    </header>

    <form @submit.prevent="handleSubmit"
      class="flex flex-col gap-3 p-8 mt-20 items-center border-2 border-solid rounded-lg m-auto w-2/5 bg-slate-100 shadow-md">
      <input v-model="username" placeholder="Username" required
        class="text-center w-2/3 shadow-lg hover:shadow-indigo-500/40 " />
      <input v-model="password" type="password" placeholder="Password" required
        class="text-center w-2/3 shadow-lg hover:shadow-indigo-500/40" />
      <button type="submit" class="w-2/3 shadow-lg hover:shadow-indigo-500/40">{{
        isLogin ? 'Вход' : 'Регистрация' }}</button>
      <button type="button" @click="autoChange" class="w-2/3 shadow-lg hover:shadow-indigo-500/40">
        {{ isLogin ? 'Нет аккаунта? Зарегистрироваться' : 'Вы зарегистрированы? Войдите в аккаунт' }}
      </button>
    </form>
  </div>
</template>

<script>

export default {
  name: 'RegisterLogin',
  сomponents: {
  },
  data() {
    return {
      username: '',
      password: '',
      isLogin: true,
    };
  },
  methods: {
    autoChange() {
      this.isLogin = !this.isLogin;
    },
    async handleSubmit() {
      if (this.isLogin) {
        await this.login();
      } else {
        await this.register();
      }
    },
    async register() {
      console.log('Регистрация: ', this.username, this.password);

      try {
        const response = await fetch(`http://localhost:5157/User/register?username=${this.username}&password=${this.password}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (!response.ok) {
          const errorText = await response.text();
          throw new Error(errorText);
        }

        const text = await response.text();
        console.log(text);
        await this.login();
      } catch (error) {
        alert('Ошибка регистрации: ' + error.message);
      }
    },
    async login() {
      console.log('Вход: ', this.username, this.password);

      try {
        const response = await fetch(`http://localhost:5157/User/login?username=${this.username}&password=${this.password}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData);
        }

        const json = await response.json();
        sessionStorage.setItem("token", json.token);
        this.goToAccount();
      } catch (error) {
        alert('Ошибка входа: ' + error.message);
      }
    },
    goToAccount() {
      console.log(this.$router);
      this.$router.push('/accountcontroller');
    }
  }
};
</script>


<style scoped>
input {
  background-color: #f0f0f5;
  border: 1px solid #d1d1d6;
  border-radius: 8px;
  padding: 12px;
  font-size: 16px;
  margin-bottom: 16px;
  transition: border-color 0.3s ease;
}

input:focus {
  border-color: #007aff;
  outline: none;
}

button {
  background-color: #007aff;
  border: none;
  border-radius: 8px;
  color: #ffffff;
  padding: 12px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin-bottom: 10px;
}

button:hover {
  background-color: #005bb5;
}
</style>