<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiService } from '@/api'
import type { CreateBoostOrderRequest } from '@/types'

const router = useRouter()
const loading = ref(false)
const error = ref('')

const form = ref<CreateBoostOrderRequest>({
  description: '',
  isParty: false,
  isPriority: false,
  steamUsername: '',
  steamPassword: '',
  startRating: 0,
  requiredRating: 0
})

const formValid = ref(false)

const rules = {
  required: (v: any) => !!v || 'This field is required',
  minRating: (v: number) => v >= 0 || 'Rating must be positive',
  maxRating: (v: number) => v <= 12000 || 'Rating must be less than 12000',
  validRange: () => {
    if (form.value.requiredRating <= form.value.startRating) {
      return 'Target rating must be higher than start rating'
    }
    return true
  },
  minLength: (min: number) => (v: string) => 
    (v && v.length >= min) || `Must be at least ${min} characters`
}

const mmrBrackets = [
  { title: 'Herald', range: '0 - 770', min: 0, max: 770 },
  { title: 'Guardian', range: '770 - 1540', min: 770, max: 1540 },
  { title: 'Crusader', range: '1540 - 2310', min: 1540, max: 2310 },
  { title: 'Archon', range: '2310 - 3080', min: 2310, max: 3080 },
  { title: 'Legend', range: '3080 - 3850', min: 3080, max: 3850 },
  { title: 'Ancient', range: '3850 - 4620', min: 3850, max: 4620 },
  { title: 'Divine', range: '4620 - 5420', min: 4620, max: 5420 },
  { title: 'Immortal', range: '5420+', min: 5420, max: 12000 }
]

async function submitOrder() {
  if (!formValid.value) return
  
  loading.value = true
  error.value = ''
  
  try {
    const order = await apiService.createBoostOrder(form.value)
    router.push({ name: 'order-details', params: { id: order.id } })
  } catch (e: any) {
    error.value = e.response?.data?.message || 'Failed to create order'
    console.error('Failed to create order:', e)
  } finally {
    loading.value = false
  }
}

function setRatingFromBracket(bracket: typeof mmrBrackets[0], isStart: boolean) {
  if (isStart) {
    form.value.startRating = bracket.min
  } else {
    form.value.requiredRating = bracket.max
  }
}

function calculateEstimatedCost(): number {
  const ratingDiff = form.value.requiredRating - form.value.startRating
  const basePrice = 2
  const priorityMultiplier = form.value.isPriority ? 1.5 : 1
  const partyMultiplier = form.value.isParty ? 1.3 : 1
  
  return Math.round(ratingDiff * basePrice * priorityMultiplier * partyMultiplier)
}

function getRatingBracket(rating: number): string {
  const bracket = mmrBrackets.find(b => rating >= b.min && rating <= b.max)
  return bracket ? bracket.title : 'Unknown'
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" lg="8" class="mx-auto">
        <v-card class="pa-6">
          <v-card-title class="text-h4 font-weight-bold mb-4">
            Create Boost Order
          </v-card-title>
          
          <v-card-subtitle class="text-body-1 mb-6">
            Fill out the details for your Dota 2 boost order. Our professional boosters will help you reach your target MMR.
          </v-card-subtitle>

          <v-form v-model="formValid" @submit.prevent="submitOrder">
            <!-- Order Description -->
            <v-row>
              <v-col cols="12">
                <v-textarea
                  v-model="form.description"
                  label="Order Description"
                  placeholder="Describe any specific requirements, preferred playtime, or additional notes..."
                  :rules="[rules.required, rules.minLength(10)]"
                  variant="outlined"
                  rows="3"
                />
              </v-col>
            </v-row>

            <!-- Steam Account Info -->
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="form.steamUsername"
                  label="Steam Username"
                  :rules="[rules.required]"
                  variant="outlined"
                  prepend-inner-icon="mdi-steam"
                />
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="form.steamPassword"
                  label="Steam Password"
                  type="password"
                  :rules="[rules.required]"
                  variant="outlined"
                  prepend-inner-icon="mdi-lock"
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  <v-icon icon="mdi-shield-check" size="small" class="mr-1" />
                  Your credentials are encrypted and secure
                </div>
              </v-col>
            </v-row>

            <!-- MMR Settings -->
            <v-row>
              <v-col cols="12">
                <h3 class="text-h6 mb-4">MMR Range</h3>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="form.startRating"
                  label="Current MMR"
                  type="number"
                  :rules="[rules.required, rules.minRating, rules.maxRating]"
                  variant="outlined"
                />
                <div class="text-caption text-grey-darken-1">
                  Current rank: {{ getRatingBracket(form.startRating) }}
                </div>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="form.requiredRating"
                  label="Target MMR"
                  type="number"
                  :rules="[rules.required, rules.minRating, rules.maxRating, rules.validRange]"
                  variant="outlined"
                />
                <div class="text-caption text-grey-darken-1">
                  Target rank: {{ getRatingBracket(form.requiredRating) }}
                </div>
              </v-col>
            </v-row>

            <!-- MMR Bracket Helper -->
            <v-row>
              <v-col cols="12">
                <v-card variant="outlined" class="pa-4">
                  <v-card-title class="text-h6 mb-3">MMR Brackets Reference</v-card-title>
                  <v-row>
                    <v-col
                      v-for="bracket in mmrBrackets"
                      :key="bracket.title"
                      cols="6"
                      sm="4"
                      md="3"
                    >
                      <v-card
                        variant="outlined"
                        class="pa-2 text-center cursor-pointer hover-card"
                        @click="setRatingFromBracket(bracket, true)"
                      >
                        <div class="font-weight-bold">{{ bracket.title }}</div>
                        <div class="text-caption">{{ bracket.range }}</div>
                      </v-card>
                    </v-col>
                  </v-row>
                </v-card>
              </v-col>
            </v-row>

            <!-- Boost Options -->
            <v-row>
              <v-col cols="12">
                <h3 class="text-h6 mb-4">Boost Options</h3>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-switch
                  v-model="form.isParty"
                  label="Party Boost"
                  color="primary"
                  inset
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  Play together with the booster (+30% cost)
                </div>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-switch
                  v-model="form.isPriority"
                  label="Priority Boost"
                  color="primary"
                  inset
                />
                <div class="text-caption text-grey-darken-1 mt-1">
                  Get your order completed faster (+50% cost)
                </div>
              </v-col>
            </v-row>

            <!-- Cost Estimation -->
            <v-row v-if="form.startRating && form.requiredRating && form.requiredRating > form.startRating">
              <v-col cols="12">
                <v-card color="primary" variant="flat" class="pa-4">
                  <v-row>
                    <v-col cols="12" md="6">
                      <h4 class="text-h6 text-white">Order Summary</h4>
                      <div class="text-white opacity-90">
                        <div>MMR Increase: {{ form.requiredRating - form.startRating }} points</div>
                        <div>{{ getRatingBracket(form.startRating) }} → {{ getRatingBracket(form.requiredRating) }}</div>
                        <div v-if="form.isParty">Party Boost: +30%</div>
                        <div v-if="form.isPriority">Priority: +50%</div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6" class="text-md-right">
                      <h4 class="text-h5 text-white">Estimated Cost</h4>
                      <div class="text-h4 font-weight-bold text-white">
                        ${{ calculateEstimatedCost() }}
                      </div>
                      <div class="text-caption text-white opacity-90">
                        Final price may vary
                      </div>
                    </v-col>
                  </v-row>
                </v-card>
              </v-col>
            </v-row>

            <!-- Error Alert -->
            <v-row v-if="error">
              <v-col cols="12">
                <v-alert
                  type="error"
                  variant="tonal"
                >
                  {{ error }}
                </v-alert>
              </v-col>
            </v-row>

            <!-- Submit Button -->
            <v-row>
              <v-col cols="12" class="d-flex gap-4">
                <v-btn
                  type="submit"
                  color="primary"
                  size="large"
                  :loading="loading"
                  :disabled="!formValid"
                  class="px-8"
                >
                  Create Order
                </v-btn>
                
                <v-btn
                  variant="outlined"
                  size="large"
                  :to="{ name: 'orders' }"
                  class="px-8"
                >
                  Cancel
                </v-btn>
              </v-col>
            </v-row>
          </v-form>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.hover-card {
  transition: all 0.2s ease-in-out;
}

.hover-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.gap-4 {
  gap: 16px;
}
</style>