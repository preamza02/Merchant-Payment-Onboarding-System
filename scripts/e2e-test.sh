#!/bin/bash
# ============================================================
# END-TO-END API TEST SCRIPT
# ============================================================
# Run this script to test all API endpoints
# Usage: ./scripts/e2e-test.sh
# ============================================================

BASE_URL="${BASE_URL:-http://localhost:5000}"

echo "🚀 Testing Merchant Payment API"
echo "Base URL: $BASE_URL"
echo "=========================================="

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

# 1. Health Check
echo ""
echo "1️⃣  Health Check..."
HEALTH=$(curl -s "$BASE_URL/health")
if echo "$HEALTH" | grep -q "Healthy"; then
    echo -e "${GREEN}✅ API is healthy${NC}"
else
    echo -e "${RED}❌ API is not healthy${NC}"
    echo "$HEALTH"
    exit 1
fi

# 2. Register User
echo ""
echo "2️⃣  Register User..."
REGISTER_RESP=$(curl -s -X POST "$BASE_URL/api/auth/register" \
    -H "Content-Type: application/json" \
    -d '{"username":"e2etest","email":"e2e@test.com","password":"Test123!"}')

if echo "$REGISTER_RESP" | grep -q "success"; then
    echo -e "${GREEN}✅ User registered${NC}"
elif echo "$REGISTER_RESP" | grep -q "already"; then
    echo -e "${GREEN}✅ User already exists (OK)${NC}"
else
    echo -e "${RED}❌ Registration failed${NC}"
    echo "$REGISTER_RESP"
fi

# 3. Login
echo ""
echo "3️⃣  Login..."
LOGIN_RESP=$(curl -s -X POST "$BASE_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d '{"username":"e2etest","password":"Test123!"}')

TOKEN=$(echo "$LOGIN_RESP" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -n "$TOKEN" ]; then
    echo -e "${GREEN}✅ Login successful${NC}"
    echo "   Token: ${TOKEN:0:40}..."
else
    echo -e "${RED}❌ Login failed${NC}"
    echo "$LOGIN_RESP"
    exit 1
fi

# 4. Create Merchant
echo ""
echo "4️⃣  Create Merchant..."
MERCHANT_RESP=$(curl -s -X POST "$BASE_URL/api/merchants" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"businessName":"E2E Test Shop","email":"e2eshop@test.com","phone":"+66899999999","address":"Test Street","taxId":"9999999999999"}')

MERCHANT_ID=$(echo "$MERCHANT_RESP" | grep -o '"merchantId":"[^"]*"' | cut -d'"' -f4)

if [ -n "$MERCHANT_ID" ]; then
    echo -e "${GREEN}✅ Merchant created${NC}"
    echo "   ID: $MERCHANT_ID"
else
    echo -e "${RED}❌ Merchant creation failed${NC}"
    echo "$MERCHANT_RESP"
fi

# 5. Get Merchant
echo ""
echo "5️⃣  Get Merchant..."
GET_MERCHANT=$(curl -s "$BASE_URL/api/merchants/$MERCHANT_ID" \
    -H "Authorization: Bearer $TOKEN")

if echo "$GET_MERCHANT" | grep -q "E2E Test Shop"; then
    echo -e "${GREEN}✅ Merchant retrieved${NC}"
else
    echo -e "${RED}❌ Get merchant failed${NC}"
fi

# 6. Create Payment
echo ""
echo "6️⃣  Create Payment..."
PAYMENT_RESP=$(curl -s -X POST "$BASE_URL/api/payments" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"merchantId\":\"$MERCHANT_ID\",\"amount\":250.50,\"currency\":\"THB\",\"description\":\"E2E Test Payment\"}")

TX_ID=$(echo "$PAYMENT_RESP" | grep -o '"transactionId":"[^"]*"' | cut -d'"' -f4)

if [ -n "$TX_ID" ]; then
    echo -e "${GREEN}✅ Payment created${NC}"
    echo "   Transaction ID: $TX_ID"
else
    echo -e "${RED}❌ Payment creation failed${NC}"
    echo "$PAYMENT_RESP"
fi

# 7. Get Payment (should be Pending)
echo ""
echo "7️⃣  Get Payment (Status: Pending)..."
GET_PAYMENT=$(curl -s "$BASE_URL/api/payments/$TX_ID" \
    -H "Authorization: Bearer $TOKEN")

if echo "$GET_PAYMENT" | grep -q "Pending"; then
    echo -e "${GREEN}✅ Payment is Pending${NC}"
else
    echo -e "${RED}❌ Unexpected payment status${NC}"
    echo "$GET_PAYMENT"
fi

# 8. Process Callback
echo ""
echo "8️⃣  Process Callback (Success)..."
CALLBACK_RESP=$(curl -s -X POST "$BASE_URL/api/payments/callback" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"transactionId\":\"$TX_ID\",\"result\":\"Success\",\"externalReferenceId\":\"E2E-BANK-001\",\"message\":\"Payment approved\"}")

if echo "$CALLBACK_RESP" | grep -q "Success"; then
    echo -e "${GREEN}✅ Callback processed${NC}"
else
    echo -e "${RED}❌ Callback failed${NC}"
    echo "$CALLBACK_RESP"
fi

# 9. Get Payment (should be Success)
echo ""
echo "9️⃣  Get Payment (Status: Success)..."
GET_PAYMENT2=$(curl -s "$BASE_URL/api/payments/$TX_ID" \
    -H "Authorization: Bearer $TOKEN")

if echo "$GET_PAYMENT2" | grep -q "Success"; then
    echo -e "${GREEN}✅ Payment is Success${NC}"
else
    echo -e "${RED}❌ Payment not updated${NC}"
fi

# 10. Get Merchant Transactions
echo ""
echo "🔟 Get Merchant Transactions..."
TRANSACTIONS=$(curl -s "$BASE_URL/api/payments/merchant/$MERCHANT_ID" \
    -H "Authorization: Bearer $TOKEN")

if echo "$TRANSACTIONS" | grep -q "$TX_ID"; then
    echo -e "${GREEN}✅ Transactions retrieved${NC}"
else
    echo -e "${RED}❌ Transactions not found${NC}"
fi

echo ""
echo "=========================================="
echo -e "${GREEN}🎉 All E2E tests completed!${NC}"
echo ""
