// index.js
import { createRouter, createMemoryHistory } from 'vue-router'
import RegisterLogin from '../components/registerLogin.vue'
import AccountController from '@/components/AccountController.vue'

const routes = [
  {
    path: '/',
    name: 'RegisterLogin',
    component: RegisterLogin
  },
  {
    path: '/accountcontroller',
    name: 'AccountController',
    component: AccountController
  }
]

const router = createRouter({
  history: createMemoryHistory(),
  routes
})

export default router
