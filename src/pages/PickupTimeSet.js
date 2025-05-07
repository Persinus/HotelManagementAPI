import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

const PickupTimeSet = () => {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Pickup Time Set</Text>
      <Text style={styles.subtitle}>Set your pickup time here.</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#FFA500', // Màu cam
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
  },
  subtitle: {
    fontSize: 16,
    color: 'white',
  },
});

export default PickupTimeSet; 